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
using Kztek_Model.Models.PM;
using Kztek_Service.Api.Interfaces;
using Kztek_Service.OneSignalr.Interfaces;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class ProjectService : IProjectService
    {
        private IPM_ProjectRepository _PM_ProjectRepsotitory;
        private IPM_ComponentRepository _PM_ComponentRepository;
        private IPM_WorkRepository _PM_WorkRepository;

        private IOS_PlayerService _OS_PlayerService;

        private ISY_UserRepository _SY_UserRepository;

        public ProjectService(IPM_ProjectRepository _PM_ProjectRepsotitory, IPM_ComponentRepository _PM_ComponentRepository, IPM_WorkRepository _PM_WorkRepository, IOS_PlayerService _OS_PlayerService, ISY_UserRepository _SY_UserRepository)
        {
            this._PM_ProjectRepsotitory = _PM_ProjectRepsotitory;
            this._PM_ComponentRepository = _PM_ComponentRepository;
            this._PM_WorkRepository = _PM_WorkRepository;

            this._OS_PlayerService = _OS_PlayerService;
            this._SY_UserRepository = _SY_UserRepository;
        }

        public async Task<MessageReport> CompleteComponent(PM_ComponentComplete model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                var query = new StringBuilder();
                query.AppendLine("{");

                query.AppendLine("'ProjectId': { '$eq': '" + model.ProjectId + "' }");
                query.AppendLine(", 'ComponentId': { '$eq': '" + model.ComponentId + "' }");
                query.AppendLine(", 'UserId': { '$eq': '" + model.UserId + "' }");

                query.AppendLine("}");

                var k = await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
                var obj = k.FirstOrDefault();

                //
                if (obj == null)
                {
                    result = new MessageReport(false, "Không tồn tại component với bạn");
                    return await Task.FromResult(result);
                }

                //Cập nhật
                obj.IsCompleted = true;
                obj.DateCompleted = DateTime.Now;
                obj.IsOnScheduled = true;

                //Lấy dự án
                var objProject = await GetProjectById(model.ProjectId);
                if (objProject == null)
                {
                    result = new MessageReport(false, "Component không tồn tại");
                    return await Task.FromResult(result);
                }

                //Kiểm tra có đúng hạn
                var objComponent = await GetComponentById(model.ComponentId);
                if (objComponent == null)
                {
                    result = new MessageReport(false, "Component không tồn tại");
                    return await Task.FromResult(result);
                }

                //Kiểm tra đúng hạn
                if (obj.DateCompleted > objComponent.DateEnd)
                {
                    obj.IsOnScheduled = false;
                }

                result = await UpdateWork(obj);

                //Gửi thông báo tới những người liên quan (user create, user do)
                SendMessage(objProject, objComponent, model.UserId);
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        public async Task<PM_ComponentCustomView> GetComponentsById(string componentid)
        {
            var cus = new PM_ComponentCustomView();

            var model = await _PM_ComponentRepository.GetOneById(componentid);
            if (model != null)
            {
                cus = new PM_ComponentCustomView()
                {
                    code = model.Code,
                    dateEnd = model.DateEnd.ToString("dd/MM/yyyy HH:mm"),
                    dateStart = model.DateStart.ToString("dd/MM/yyyy HH:mm"),
                    description = model.Description,
                    id = model.Id,
                    title = model.Title,
                    dateCreated = model.DateCreated.ToString("dd/MM/yyyy HH:mm"),
                    note = model.Note
                };
            }

            return cus;
        }

        public async Task<List<PM_ComponentCustomView>> GetComponentsByUserId_ProjectId(string userid, string projectid, int status = 0)
        {
            var objProject = await _PM_ProjectRepsotitory.GetOneById(projectid);

            var dataWork = await GetAllUnfinishedWorkByUserId(userid, projectid);
            var ids = dataWork.Select(n => n.ComponentId).ToList();

            var time = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            var count = 0;

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'Status': {'$eq': " + status + "}");

            query.AppendLine("'DateStart': {'$lt': ISODate('" + time + "T00:00:00.000+07:00')}");

            query.AppendLine(", '_id': { '$in': [");

            foreach (var item in ids)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == ids.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            var data = await _PM_ComponentRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            var cus = new List<PM_ComponentCustomView>();

            foreach (var item in data)
            {
                cus.Add(new PM_ComponentCustomView()
                {
                    id = item.Id,
                    code = item.Code,
                    title = item.Title,
                    description = item.Description,
                    dateStart = item.DateStart.ToString("dd/MM/yyyy HH:mm"),
                    dateEnd = item.DateEnd.ToString("dd/MM/yyyy HH:mm")
                });
            }

            return cus;
        }

        public async Task<List<PM_ProjectCustomView>> GetProjectsByUserId(string userid, int status = 0)
        {
            var dataWork = await GetAllUnfinishedWorkByUserId(userid);
            var ids = dataWork.Select(n => n.ProjectId).ToList();

            var count = 0;

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'Status': {'$eq': " + status + "}");

            query.AppendLine(", '_id': { '$in': [");

            foreach (var item in ids)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == ids.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            var data = await _PM_ProjectRepsotitory.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            var cus = new List<PM_ProjectCustomView>();

            foreach (var item in data)
            {
                cus.Add(new PM_ProjectCustomView()
                {
                    id = item.Id,
                    title = item.Title,
                    description = item.Description,
                    dateStart = item.DateStart.ToString("dd/MM/yyyy HH:mm"),
                    dateEnd = item.DateEnd.ToString("dd/MM/yyyy HH:mm")
                });
            }

            return cus;
        }

        private async Task<List<PM_Work>> GetAllUnfinishedWorkByUserId(string userid, string projectid = "")
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine(", 'IsCompleted': false");

            if (!string.IsNullOrEmpty(projectid))
            {
                query.AppendLine(", 'ProjectId': { '$eq': '" + projectid + "' }");
            }

            query.AppendLine("}");

            return await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        private async Task<PM_Project> GetProjectById(string id)
        {
            return await _PM_ProjectRepsotitory.GetOneById(id);
        }

        private async Task<PM_Component> GetComponentById(string id)
        {
            return await _PM_ComponentRepository.GetOneById(id);
        }

        private async Task<SY_User> GetUserById(string id)
        {
            return await _SY_UserRepository.GetOneById(id);
        }

        private async Task<MessageReport> UpdateWork(PM_Work model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _PM_WorkRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }

        private async Task<MessageReport> SendMessage(PM_Project project, PM_Component component, string userid)
        {
            var result = new MessageReport(false, "error");

            try
            {
                //Lấy danh sách người dùng liên quan tới component + người tạo dự án
                var users = await GetUserInvolved(component.Id);
                users.Add(project.UserCreatedId);

                //Lấy người hoàn thiện
                var objUser = await GetUserById(userid);

                //Lấy Players 
                var players = await _OS_PlayerService.GetPlayerIdsByUserIds(users);

                //Gửi
                var model = new OneSignalrMessage()
                {
                    Id = "",
                    Title = string.Format("Dự án: {0}", project.Title),
                    Description = string.Format("Component: {0} được cập nhật hoàn thành bởi {1}", component.Code, objUser != null ? objUser.Username : ""),
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
    }
}
