using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.MN;

namespace Kztek_Service.Admin.Interfaces.MN
{
    public interface IMN_LicenseService
    {
        Task<GridModel<MN_License>> GetPaging(string key,string fromdate, string todate, int pageNumber, int pageSize);

        Task<MN_License> GetById(string id);

        Task<MN_License> GetByName(string name);

        Task<MessageReport> Create(MN_License model);

        Task<MessageReport> Update(MN_License model);

        Task<MessageReport> Delete(string id);
        Task<GridModel<MN_License>> GetPagings(string key, string fromdate, string todate, int page, int pagesize);
    }
}
