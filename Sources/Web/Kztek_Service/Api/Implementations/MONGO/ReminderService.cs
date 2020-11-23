using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Model.Models.PM;
using Kztek_Model.Models.WM;
using Kztek_Service.Api.Interfaces;
using Kztek_Service.OneSignalr.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class ReminderService : IReminderService
    {
        private IPM_ProjectRepository _PM_ProjectRepsotitory;
        private IPM_ComponentRepository _PM_ComponentRepository;
        private IPM_WorkRepository _PM_WorkRepository;
        private IWM_TaskRepository _WM_TaskRepository;
        private IWM_TaskUserRepository _WM_TaskUserRepository;
        private IOS_PlayerService _OS_PlayerService;
        private ISY_ReminderSystemRepository _SY_ReminderSystemRepository;

        public ReminderService(IPM_ProjectRepository _PM_ProjectRepsotitory, IPM_ComponentRepository _PM_ComponentRepository, IPM_WorkRepository _PM_WorkRepository, IWM_TaskRepository _WM_TaskRepository, IWM_TaskUserRepository _WM_TaskUserRepository, IOS_PlayerService _OS_PlayerService, ISY_ReminderSystemRepository _SY_ReminderSystemRepository)
        {
            this._PM_ProjectRepsotitory = _PM_ProjectRepsotitory;
            this._PM_ComponentRepository = _PM_ComponentRepository;
            this._PM_WorkRepository = _PM_WorkRepository;
            this._WM_TaskRepository = _WM_TaskRepository;
            this._WM_TaskUserRepository = _WM_TaskUserRepository;
            this._OS_PlayerService = _OS_PlayerService;
            this._SY_ReminderSystemRepository = _SY_ReminderSystemRepository;
        }

        public async Task<MessageReport> ReminderToClient(ReminderModel model)
        {
            var result = new MessageReport(false, "error");

            switch (model.RecordType)
            {
                case "project":
                    return await ReminderProject(model.RecordId, model.Decription);

                case "task":
                    return await ReminderTask(model.RecordId, model.Decription);

                default:
                    break;
            }

            //Cập nhật trong log
            var mo = await GetLogById(model.RecordId);
            if (mo != null)
            {
                mo.isDone = true;

                await UpdateLog(mo);
            }

            return await Task.FromResult(result);
        }

        private async Task<MessageReport> ReminderProject(string id, string message)
        {
            var result = new MessageReport(false, "error");

            //Lấy component của dự án
            var objComponent = await GetComponentById(id);
            if (objComponent == null)
            {
                result = new MessageReport(false, "Component không tồn tại");
                return result;
            }

            //Lấy dự án
            var objProject = await GetProjectById(objComponent.ProjectId);
            if (objProject == null)
            {
                result = new MessageReport(false, "Project không tồn tại");
                return result;
            }

            //Gửi thông báo
            result = await SendMessageProject(objProject, objComponent);

            return result;
        }

        private async Task<MessageReport> ReminderTask(string id, string message)
        {
            var result = new MessageReport(false, "error");

            //Lấy task
            var objTask = await GetTaskById(id);
            if (objTask == null)
            {
                result = new MessageReport(false, "Task không tồn tại");
                return result;
            }

            //GỬi thông báo
            result = await SendMessageTask(objTask);

            return result;
        }

        private async Task<PM_Project> GetProjectById(string id)
        {
            return await _PM_ProjectRepsotitory.GetOneById(id);
        }

        private async Task<PM_Component> GetComponentById(string id)
        {
            return await _PM_ComponentRepository.GetOneById(id);
        }

        private async Task<WM_Task> GetTaskById(string id)
        {
            return await _WM_TaskRepository.GetOneById(id);
        }

        private async Task<List<string>> GetUserTaskInvolved(string taskid)
        {
            var list = new List<string>();

            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'TaskId': { '$eq': '" + taskid + "' }");
            query.AppendLine("}");

            var k = await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            var users = k.Select(n => n.UserId).Distinct().ToList();

            foreach (var item in users)
            {
                list.Add(item);
            }

            return list;
        }


        private async Task<List<string>> GetUserInvolved(string componentid)
        {
            var list = new List<string>();

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ComponentId': { '$eq': '" + componentid + "' }");

            query.AppendLine("}");

            var k = await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            var users = k.Select(n => n.UserId).Distinct().ToList();

            foreach (var item in users)
            {
                list.Add(item);
            }

            return list;
        }

        private async Task<MessageReport> SendMessageProject (PM_Project project, PM_Component component)
        {
            var result = new MessageReport(false, "error");

            try
            {
                //Lấy danh sách người dùng liên quan tới component + người tạo dự án
                var users = await GetUserInvolved(component.Id);
                users.Add(project.UserCreatedId);

                //Lấy Players 
                var players = await _OS_PlayerService.GetPlayerIdsByUserIds(users);

                //Gửi
                var model = new OneSignalrMessage()
                {
                    Id = "",
                    Title = string.Format("Dự án: {0}", project.Title),
                    Description = string.Format("Nhắc nhở hoàn thành Component: {0}", component.Code),
                    UserIds = "",
                    PlayerIds = players.Select(n => n.PlayerId).ToArray(),
                    View = "HomePage"
                };

                result = await _OS_PlayerService.SendMessage(model);
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        private async Task<MessageReport> SendMessageTask(WM_Task task)
        {
            var result = new MessageReport(false, "error");

            try
            {
                //Lấy danh sách người dùng liên quan tới task + người tạo dự án
                var users = await GetUserTaskInvolved(task.Id);
                users.Add(task.UserCreatedId);

                //Lấy Players 
                var players = await _OS_PlayerService.GetPlayerIdsByUserIds(users);

                //Gửi
                var model = new OneSignalrMessage()
                {
                    Id = "",
                    Title = string.Format("Nhắc nhở công việc: {0}", task.Title),
                    Description = "Công việc sắp đến hạn hoàn thành",
                    UserIds = "",
                    PlayerIds = players.Select(n => n.PlayerId).ToArray(),
                    View = "TaskPage"
                };

                result = await _OS_PlayerService.SendMessage(model);
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<List<SY_ReminderSystem>> DataNeedReminder()
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'isDone': false");

            query.AppendLine("}");

            return await _SY_ReminderSystemRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        private async Task<SY_ReminderSystem> GetLogById(string id)
        {
            return await _SY_ReminderSystemRepository.GetOneById(id);
        }

        private async Task<MessageReport> UpdateLog(SY_ReminderSystem model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _SY_ReminderSystemRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
