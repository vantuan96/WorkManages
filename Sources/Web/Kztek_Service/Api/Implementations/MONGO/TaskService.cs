using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Model.Models.WM;
using Kztek_Service.Api.Interfaces;
using Kztek_Service.OneSignalr.Interfaces;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class TaskService : ITaskService
    {
        private IWM_TaskRepository _WM_TaskRepository;
        private IWM_TaskUserRepository _WM_TaskUserRepository;

        private IOS_PlayerService _OS_PlayerService;

        private ISY_UserRepository _SY_UserRepository;

        public TaskService(IWM_TaskRepository _WM_TaskRepository, IWM_TaskUserRepository _WM_TaskUserRepository, IOS_PlayerService _OS_PlayerService, ISY_UserRepository _SY_UserRepository)
        {
            this._WM_TaskRepository = _WM_TaskRepository;
            this._WM_TaskUserRepository = _WM_TaskUserRepository;
            this._OS_PlayerService = _OS_PlayerService;
            this._SY_UserRepository = _SY_UserRepository;
        }

        public async Task<MessageReport> CompleteTask(WM_TaskComplete model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Lấy task
                var objTask = await GetTaskById(model.TaskId);
                if (objTask == null)
                {
                    result = new MessageReport(false, "Công việc của bạn không tồn tại");
                    return result;
                }

                //Task user
                var userTask = await GetTaskUserByTaskId_UserId(model.TaskId, model.UserId);
                if (userTask == null)
                {
                    result = new MessageReport(false, "Công việc của bạn không tồn tại");
                    return result;
                }

                //Check công việc hoàn thành
                userTask.IsCompleted = true;
                userTask.DateCompleted = DateTime.Now;
                userTask.IsOnScheduled = true;

                if (userTask.DateCompleted > objTask.DateEnd)
                {
                    userTask.IsOnScheduled = false;
                }

                result = await UpdateUserTask(userTask);
                if (result.isSuccess)
                {
                    //danh sách 
                    var userTasks = await GetTaskUsersByTaskId(objTask.Id);
                    var user = userTasks.Select(n => n.UserId).ToList();
                    user.Add(objTask.UserCreatedId);

                    SendMessage(objTask, user, model.UserId);

                    RemoveSchedule(objTask);
                }
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<List<WM_TaskCustomView>> GetCurrentTasksByUserId(string UserId)
        {
            var count = 0;

            //Lấy danh sách công việc được phân
            var userTasks = await GetTaskUserByUserId(UserId);

            //Lấy ra danh sách task đi theo công việc đã phân
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'_id': { '$in': [");

            foreach (var item in userTasks)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item.TaskId, count == userTasks.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            var data = await _WM_TaskRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            var custom = new List<WM_TaskCustomView>();

            foreach (var item in data)
            {
                custom.Add(new WM_TaskCustomView()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    DateStart = item.DateStart.ToString("dd/MM/yyyy HH:mm"),
                    DateEnd = item.DateEnd.ToString("dd/MM/yyyy HH:mm"),
                    Status = item.Status
                });
            }

            return custom;
        }

        private async Task<List<WM_TaskUser>> GetTaskUserByUserId(string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine(", 'IsCompleted': false");
            query.AppendLine("}");

            return await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        private async Task<WM_Task> GetTaskById(string id)
        {
            return await _WM_TaskRepository.GetOneById(id);
        }

        private async Task<WM_TaskUser> GetTaskUserByTaskId_UserId(string taskid, string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'TaskId': { '$eq': '" + taskid + "' }");
            query.AppendLine(", 'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine("}");

            var k = await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return k.FirstOrDefault();
        }

        private async Task<List<WM_TaskUser>> GetTaskUsersByTaskId(string taskid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'TaskId': { '$eq': '" + taskid + "' }");
            query.AppendLine("}");

            var k = await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return k;
        }

        private async Task<MessageReport> UpdateUserTask(WM_TaskUser model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _WM_TaskUserRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }

        private async Task<SY_User> GetUserById(string id)
        {
            return await _SY_UserRepository.GetOneById(id);
        }

        private async Task<MessageReport> SendMessage(WM_Task task, List<string> users, string userid)
        {
            var result = new MessageReport(false, "error");

            try
            {
                //Người check hoàn thành
                var user = await GetUserById(userid);

                //Lấy Players 
                var players = await _OS_PlayerService.GetPlayerIdsByUserIds(users);

                //Gửi
                var model = new OneSignalrMessage()
                {
                    Id = "",
                    Title = string.Format("Công việc: {0}", task.Title),
                    Description = string.Format("Công việc được check hoàn thành bởi {0}", user.Username),
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

        private async Task RemoveSchedule(WM_Task model)
        {
            var paramPs = new Dictionary<string, string>();
            paramPs.Add("RecordId", model.Id);
            paramPs.Add("RecordType", "task");

            var respond = await ApiHelper.HttpPostFormData("http://localhost:8888/api/wm_task/removeregisterreminder", paramPs, "");
        }
    }
}