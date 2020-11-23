using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.PM;
using Kztek_Service.Admin.Interfaces.PM;
using MongoDB.Driver;

namespace Kztek_Service.Admin.Implementations.MONGO.PM
{
    public class PM_WorkService : IPM_WorkService
    {
        private IPM_WorkRepository _PM_WorkRepository;

        public PM_WorkService(IPM_WorkRepository _PM_WorkRepository)
        {
            this._PM_WorkRepository = _PM_WorkRepository;
        }

        public async Task<MessageReport> Create(PM_Work model)
        {
            return await _PM_WorkRepository.Add(model);
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

                return await _PM_WorkRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> DeleteByComponentId(string componentid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ComponentId': { '$eq': '" + componentid + "' }");

            query.AppendLine("}");

            return await _PM_WorkRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<MessageReport> DeleteByProjectId(string projectid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ProjectId': { '$eq': '" + projectid + "' }");

            query.AppendLine("}");

            return await _PM_WorkRepository.Remove_Multi(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Work>> GetAllByComponentId(string componentid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ComponentId': { '$eq': '" + componentid + "' }");

            query.AppendLine("}");

            return await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Work>> GetAllByProjectId(string projectid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ProjectId': { '$eq': '" + projectid + "' }");

            query.AppendLine("}");

            return await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Work>> GetAllUnfinishedWorkByUserId(string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine(", 'IsCompleted': false");

            query.AppendLine("}");

            return await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<PM_Work>> GetAllUnfinishedWorkByUserId_ProjectId(string userid, string projectid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ProjectId': { '$eq': '" + projectid + "' }");
            query.AppendLine(", 'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine(", 'IsCompleted': false");

            query.AppendLine("}");

            return await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<PM_Work> GetById(string id)
        {
            return await _PM_WorkRepository.GetOneById(id);
        }

        public async Task<PM_Work> GetByProjectId_ComponentId_UserId(string projectid, string componentid, string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'ProjectId': { '$eq': '" + projectid + "' }");
            query.AppendLine(", 'ComponentId': { '$eq': '" + componentid + "' }");
            query.AppendLine(", 'UserId': { '$eq': '" + userid + "' }");

            query.AppendLine("}");

            var k = await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return k.FirstOrDefault();
        }

        public async Task<MessageReport> MCompleteByComponentId(string componentid)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                var query = new StringBuilder();
                query.AppendLine("{");

                query.AppendLine("'ComponentId': { '$eq': '" + componentid + "' }");

                query.AppendLine("}");

                var k = await _PM_WorkRepository.GetCollection();

                var update = Builders<PM_Work>.Update
                            .Set(p => p.DateCompleted, DateTime.Now)
                            .Set(p => p.IsCompleted, true)
                            .Set(p => p.IsOnScheduled, true);

                var a = await k.UpdateManyAsync(MongoHelper.ConvertQueryStringToDocument(query.ToString()), update);

                result = new MessageReport(true, "Thành công");
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<MessageReport> Update(PM_Work model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _PM_WorkRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
