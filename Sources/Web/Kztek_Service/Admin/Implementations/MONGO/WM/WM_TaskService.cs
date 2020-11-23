using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;
using Kztek_Service.Admin.Interfaces.WM;
using Kztek_Service.OneSignalr.Interfaces;

namespace Kztek_Service.Admin.Implementations.MONGO.WM
{
    public class WM_TaskService : IWM_TaskService
    {
        private IWM_TaskRepository _WM_TaskRepository;
        private IWM_TaskUserRepository _WM_TaskUserRepository;

        private IOS_PlayerService _OS_PlayerService;

        public WM_TaskService(IWM_TaskRepository _WM_TaskRepository, IWM_TaskUserRepository _WM_TaskUserRepository, IOS_PlayerService _OS_PlayerService)
        {
            this._WM_TaskRepository = _WM_TaskRepository;
            this._WM_TaskUserRepository = _WM_TaskUserRepository;
            this._OS_PlayerService = _OS_PlayerService;
        }

        public async Task<MessageReport> Create(WM_Task model)
        {
            return await _WM_TaskRepository.Add(model);
        }

        public async Task<MessageReport> Delete(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await GetById(id);
            if (obj != null)
            {
                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + id + "' }");
                query.AppendLine("}");

                result = await _WM_TaskRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

                if (result.isSuccess)
                {
                    await DeleteUserTasksByTaskId(id);
                }

                return result;
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<WM_Task> GetById(string id)
        {
            return await _WM_TaskRepository.GetOneById(id);
        }

        public async Task<WM_TaskSubmit> GetCustomById(string id)
        {
            var model = new WM_TaskSubmit();

            var obj = await _WM_TaskRepository.GetOneById(id);
            if (obj != null)
            {
                model = await GetCustomByModel(obj);
            }

            return model;
        }

        public async Task<WM_TaskSubmit> GetCustomByModel(WM_Task model)
        {
            var obj = new WM_TaskSubmit()
            {
                Id = model.Id,
                DateEnd = model.DateEnd.ToString("dd/MM/yyyy HH:mm"),
                DateStart = model.DateStart.ToString("dd/MM/yyyy HH:mm"),
                Description = model.Description,
                Title = model.Title,
                Status = model.Status,
                Note = ""
            };

            var userTasks = from n in _WM_TaskUserRepository.Table
                            where n.TaskId == model.Id
                            select n;

            obj.UserTasks = userTasks.ToList();
            obj.UserIds = userTasks.Select(n => n.UserId).ToList();

            return await Task.FromResult(obj);
        }

        public async Task<GridModel<WM_Task>> GetPaging(string key, int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(key))
            {
                query.AppendLine("'$or': [");

                query.AppendLine("{ 'Title': { '$in': [/" + key + "/i] } }");
                query.AppendLine(", { 'Description': { '$in': [/" + key + "/i] } }");

                query.AppendLine("]");
            }

            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'Status': 1");
            sort.AppendLine(", 'DateCreated': -1");
            sort.AppendLine("}");

            return await _WM_TaskRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);
        }

        public async Task<MessageReport> Update(WM_Task model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _WM_TaskRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }

        public async Task<MessageReport> DeleteUserTasksByTaskId(string taskid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'TaskId': { '$eq': '" + taskid + "' }");
            query.AppendLine("}");

            return await _WM_TaskUserRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<MessageReport> CreateUserTask(WM_TaskUser model)
        {
            return await _WM_TaskUserRepository.Add(model);
        }

        public async Task<List<WM_TaskUser>> GetUserTasksByTaskId(string taskid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'TaskId': { '$eq': '" + taskid + "' }");
            query.AppendLine("}");

            return await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<WM_TaskUser>> GetUserTasksByUserId(string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine(", 'IsCompleted': false");
            query.AppendLine("}");

            return await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<WM_Task>> GetTaskByUserId(string userid)
        {
            var count = 0;

            //Danh sách công việc của người đó
            var dt = await GetUserTasksByUserId(userid);

            //Lấy ra danh sách task đi theo công việc đã phân
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'_id': { '$in': [");

            foreach (var item in dt)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item.TaskId, count == dt.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            return await _WM_TaskRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<WM_TaskUser> GetByTaskId_UserId(string taskid, string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'TaskId': { '$eq': '" + taskid + "' }");
            query.AppendLine(", 'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine("}");

            var k = await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return k.FirstOrDefault();
        }

        public async Task<MessageReport> UpdateUserTask(WM_TaskUser model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _WM_TaskUserRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }

        private async Task<MessageReport> DeleteTaskUserById(string taskid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'TaskId': { '$eq': '" + taskid + "' }");
            query.AppendLine("}");

            return await _WM_TaskUserRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }
    }
}