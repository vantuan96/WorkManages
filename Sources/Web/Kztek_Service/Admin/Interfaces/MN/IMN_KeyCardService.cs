using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.MN;

namespace Kztek_Service.Admin.Interfaces.MN
{
    public interface IMN_KeyCardService
    {
        Task<GridModel<MN_KeyCard>> GetPaging(string key, string keysecurityid, string fromdate, string todate, int pageNumber, int pageSize);
    }
}
