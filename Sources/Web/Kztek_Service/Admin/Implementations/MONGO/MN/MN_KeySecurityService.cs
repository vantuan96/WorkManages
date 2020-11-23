using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;

namespace Kztek_Service.Admin.Implementations.MONGO.MN
{
    public class MN_KeySecurityService : IMN_KeySecurityService
    {
        private IMN_KeySecurityRepository _MN_KeySecurityRepository;

        public MN_KeySecurityService(IMN_KeySecurityRepository _MN_KeySecurityRepository)
        {
            this._MN_KeySecurityRepository = _MN_KeySecurityRepository;
        }

        public async Task<MessageReport> Create(MN_KeySecurity model)
        {
            return await _MN_KeySecurityRepository.Add(model);
        }

        public async Task<MessageReport> Delete(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await GetById(id);
            if (obj != null)
            {
                //var query = new StringBuilder();
                //query.AppendLine("{");
                //query.AppendLine("'_id': { '$eq': '" + id + "' }");
                //query.AppendLine("}");

                obj.IsDeleted = true;

                return await Update(obj);
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MN_KeySecurity> GetByCode(string code)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'IsDeleted': false");
            query.AppendLine(", 'Code': { '$eq': '" + code + "' }");
            query.AppendLine("}");

            var data = await _MN_KeySecurityRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return data.FirstOrDefault();
            
        }

        public async Task<MN_KeySecurity> GetById(string id)
        {
            return await _MN_KeySecurityRepository.GetOneById(id);
        }

        public async Task<MN_KeySecurity> GetByKeyA(string keya)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'IsDeleted': false");
            query.AppendLine(", 'KeyA': { '$eq': '" + keya + "' }");
            query.AppendLine("}");

            var data = await _MN_KeySecurityRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return data.FirstOrDefault();
        }

        public async Task<MN_KeySecurity> GetByKeyB(string keyb)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'IsDeleted': false");
            query.AppendLine(", 'KeyB': { '$eq': '" + keyb + "' }");
            query.AppendLine("}");

            var data = await _MN_KeySecurityRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return data.FirstOrDefault();
        }

        public async Task<GridModel<MN_KeySecurity>> GetPaging(string key, int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'IsDeleted': false");

            if (!string.IsNullOrWhiteSpace(key))
            {
                query.AppendLine(", '$or': [");

                query.AppendLine("{ 'Name': { '$in': [/" + key + "/i] } }");
                query.AppendLine(", { 'Code': { '$in': [/" + key + "/i] } }");

                query.AppendLine("]");
            }

            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'Name': 1");
            sort.AppendLine("}");

            return await _MN_KeySecurityRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);
        }

        public async Task<MessageReport> Update(MN_KeySecurity model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _MN_KeySecurityRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
