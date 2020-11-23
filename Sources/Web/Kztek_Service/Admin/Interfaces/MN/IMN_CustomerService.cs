using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models.MN;

namespace Kztek_Service.Admin.Interfaces.MN
{
    public interface IMN_CustomerService
    {
        Task<GridModel<MN_Customer>> GetPaging(string key, string customergroupid, int pageNumber, int pageSize);

        Task<GridModel<MN_CustomerCustomView>> GetCustomPaging(string key, string customergroupid, int pageNumber, int pageSize);

        Task<MN_Customer> GetById(string id);

        Task<MessageReport> Create(MN_Customer model);

        Task<MessageReport> Update(MN_Customer model);

        Task<MessageReport> Delete(string id);
    }
}
