using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Microsoft.AspNetCore.Http;

namespace Kztek_Service.Admin.Interfaces
{
    public interface ISY_MenuFunctionService
    {
        Task<IEnumerable<SY_MenuFunction>> GetAllActiveByUserId(HttpContext context, SessionModel model);

        Task<List<SY_MenuFunction>> GetAll();

        Task<List<SY_MenuFunction>> GetAllActive();

        Task<List<SY_MenuFunction>> GetAllActiveOrder();

        Task<List<SY_MenuFunction_Submit>> GetAllCustomActiveOrder();

        Task<SY_MenuFunction> GetById(string id);

        Task<SY_MenuFunction_Submit> GetCustomById(string id);

        Task<SY_MenuFunction_Submit> GetCustomByModel(SY_MenuFunction model);

        Task<MessageReport> Create(SY_MenuFunction model);

        Task<MessageReport> Update(SY_MenuFunction model);

        Task<MessageReport> Delete(string ids);

        Task<string> GetBreadcrumb(string id, string parentid, string lastvalue);

        Task<MessageReport> CreateMap(SY_Map_Role_Menu model);

        Task<MessageReport> DeleteMap(string roleid);
    }
}