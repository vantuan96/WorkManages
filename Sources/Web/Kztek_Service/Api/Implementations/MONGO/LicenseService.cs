using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.MN;
using Kztek_Service.Api.Interfaces;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class LicenseService : ILicenseService
    {
        private IMN_LicenseRepository _MN_LicenseRepository;

        public LicenseService(IMN_LicenseRepository _MN_LicenseRepository)
        {
            this._MN_LicenseRepository = _MN_LicenseRepository;
        }

        public async Task<MN_License> License(string name)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'ProjectName': { '$eq': '" + name + "' }");
            query.AppendLine("}");

            var data = await _MN_LicenseRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return data.FirstOrDefault();
        }

        public async Task<List<MN_License>> GetAll()
        {
            var result = _MN_LicenseRepository.Table.ToList();

            return await Task.FromResult(result);
        }
    }
}
