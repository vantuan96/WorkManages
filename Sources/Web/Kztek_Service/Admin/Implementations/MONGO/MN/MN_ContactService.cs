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
    public class MN_ContactService : IMN_ContactService
    {
        private IMN_ContactRepository _MN_ContactRepository;

        public MN_ContactService(IMN_ContactRepository _MN_ContactRepository)
        {
            this._MN_ContactRepository = _MN_ContactRepository;
        }

        public async Task<MessageReport> Create(MN_Contact model)
        {
            return await _MN_ContactRepository.Add(model);
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

                return await _MN_ContactRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MN_Contact> GetById(string id)
        {
            return await _MN_ContactRepository.GetOneById(id);
        }

        public async Task<List<MN_Contact>> GetListByCustomer(string customerid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'CustomerId': { '$eq' : '" + customerid + "' }");

            query.AppendLine("}");

            return await _MN_ContactRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<MessageReport> Update(MN_Contact model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _MN_ContactRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
