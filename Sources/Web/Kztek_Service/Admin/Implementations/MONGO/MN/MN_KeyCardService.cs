using System;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;

namespace Kztek_Service.Admin.Implementations.MONGO.MN
{
    public class MN_KeyCardService : IMN_KeyCardService
    {
        private IMN_KeyCardRepository _MN_KeyCardRepository;

        public MN_KeyCardService(IMN_KeyCardRepository _MN_KeyCardRepository)
        {
            this._MN_KeyCardRepository = _MN_KeyCardRepository;
        }

        public async Task<GridModel<MN_KeyCard>> GetPaging(string key, string keysecurityid, string fromdate, string todate, int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'KeySecurityId': {'$eq': '" + keysecurityid + "'}");

            if (!string.IsNullOrWhiteSpace(key))
            {
                query.AppendLine(", '$or': [");

                query.AppendLine("{ 'CardNo': { '$in': [/" + key + "/i] } }");
                query.AppendLine(", { 'CardNumber': { '$in': [/" + key + "/i] } }");

                query.AppendLine("]");
            }

            if (!string.IsNullOrWhiteSpace(fromdate) && !string.IsNullOrWhiteSpace(todate))
            {
                fromdate = Convert.ToDateTime(fromdate).ToString("yyyy-MM-dd");
                todate = Convert.ToDateTime(todate).AddDays(1).ToString("yyyy-MM-dd");
                query.Append(", 'DateCreated' : { '$gte': ISODate('" + fromdate + "'), '$lt': ISODate('" + todate + "') }");
            }

            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'CardNo': 1");
            sort.AppendLine("}");

            return await _MN_KeyCardRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);
        }
    }
}
