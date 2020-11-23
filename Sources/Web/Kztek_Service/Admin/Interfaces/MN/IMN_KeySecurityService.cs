using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.MN;

namespace Kztek_Service.Admin.Interfaces.MN
{
    public interface IMN_KeySecurityService
    {
        Task<GridModel<MN_KeySecurity>> GetPaging(string key, int pageNumber, int pageSize);

        Task<MN_KeySecurity> GetById(string id);

        Task<MN_KeySecurity> GetByCode(string code);

        Task<MN_KeySecurity> GetByKeyA(string keya);

        Task<MN_KeySecurity> GetByKeyB(string keyb);

        Task<MessageReport> Create(MN_KeySecurity model);

        Task<MessageReport> Update(MN_KeySecurity model);

        Task<MessageReport> Delete(string id);
    }
}
