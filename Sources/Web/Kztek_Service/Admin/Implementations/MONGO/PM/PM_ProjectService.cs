using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.PM;
using Kztek_Service.Admin.Interfaces.PM;

namespace Kztek_Service.Admin.Implementations.MONGO.PM
{
    public class PM_ProjectService : IPM_ProjectService
    {
        private IPM_ProjectRepository _PM_ProjectRepository;

        public PM_ProjectService(IPM_ProjectRepository _PM_ProjectRepository)
        {
            this._PM_ProjectRepository = _PM_ProjectRepository;
        }

        public async Task<MessageReport> Create(PM_Project model)
        {
            return await _PM_ProjectRepository.Add(model);
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

                return await _PM_ProjectRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<PM_Project> GetById(string id)
        {
            return await _PM_ProjectRepository.GetOneById(id);
        }

        public async Task<GridModel<PM_Project>> GetPaging(string key, int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(key))
            {
                query.AppendLine("'$or': [");

                query.AppendLine("{ 'Name': { '$in': [/" + key + "/i] } }");
                query.AppendLine(", { 'Username': { '$in': [/" + key + "/i] } }");

                query.AppendLine("]");
            }

            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'DateCreated': -1");
            sort.AppendLine("}");

            return await _PM_ProjectRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);
        }

        public async Task<List<PM_Project>> GetProjectsByIds(List<string> ids, int status = 0)
        {
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

            return await _PM_ProjectRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<MessageReport> Update(PM_Project model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _PM_ProjectRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
