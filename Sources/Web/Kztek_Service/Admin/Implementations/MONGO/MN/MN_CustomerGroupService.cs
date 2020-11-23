using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;

namespace Kztek_Service.Admin.Implementations.MONGO.MN
{
    public class MN_CustomerGroupService : IMN_CustomerGroupService
    {
        private IMN_CustomerGroupRepository _MN_CustomerGroupRepository;

        public MN_CustomerGroupService(IMN_CustomerGroupRepository _MN_CustomerGroupRepository)
        {
            this._MN_CustomerGroupRepository = _MN_CustomerGroupRepository;
        }

        public async Task<MessageReport> Create(MN_CustomerGroup model)
        {
            return await _MN_CustomerGroupRepository.Add(model);
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

                return await _MN_CustomerGroupRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<List<MN_CustomerGroup>> GetActiveByFirst()
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("}");

            return await _MN_CustomerGroupRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<MN_CustomerGroup> GetById(string id)
        {
            return await _MN_CustomerGroupRepository.GetOneById(id);
        }

        public async Task<GridModel<MN_CustomerGroup>> GetPaging(string key, int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(key))
            {
                query.AppendLine("'$or': [");

                query.AppendLine("{ 'Name': { '$in': [/" + key + "/i] } }");
                query.AppendLine(", { 'Description': { '$in': [/" + key + "/i] } }");

                query.AppendLine("]");
            }

            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'Name': 1");
            sort.AppendLine("}");

            return await _MN_CustomerGroupRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);
        }

        public async Task<MessageReport> Update(MN_CustomerGroup model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _MN_CustomerGroupRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
