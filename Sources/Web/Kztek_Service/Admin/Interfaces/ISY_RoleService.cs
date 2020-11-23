using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Model.Models;

namespace Kztek_Service.Admin.Interfaces
{
    public interface ISY_RoleService
    {
        Task<List<SY_Role>> GetAll();

        Task<List<SY_Role>> GetAllActiveOrder();

        Task<SY_Role> GetById(string id);

        Task<SY_Role_Submit> GetCustomById(string id);

        Task<SY_Role_Submit> GetCustomByModel(SY_Role model);

        Task<MessageReport> Create(SY_Role model);

        Task<MessageReport> Update(SY_Role model);

        Task<MessageReport> Delete(string id);

        Task<MessageReport> CreateMap(SY_Map_User_Role model);

        Task<MessageReport> DeleteMap(string userid);
    }
}