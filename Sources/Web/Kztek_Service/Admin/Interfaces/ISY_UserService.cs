using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Library.Security;
using Kztek_Model.Models;
using X.PagedList;

namespace Kztek_Service.Admin.Interfaces
{
    public interface ISY_UserService
    {
        Task<List<SY_User>> GetAll();

        Task<List<SY_User>> GetAllActive();

        Task<GridModel<SY_User>> GetPaging(string key, int pageNumber, int pageSize);

        Task<SY_User> GetById(string id);

        Task<SY_User> GetByUsername(string username);

        Task<SY_User> GetByUsername_notId(string username, string id);

        Task<SY_User_Submit> GetCustomById(string id);

        Task<SY_User_Submit> GetCustomByModel(SY_User model);

        Task<MessageReport> Create(SY_User model);

        Task<MessageReport> Update(SY_User model);

        Task<MessageReport> Delete(string id);

        Task<MessageReport> Login(AuthModel model, out SY_User user);

        Task<List<SY_User>> GetAllByIds(List<string> ids);
    }
}