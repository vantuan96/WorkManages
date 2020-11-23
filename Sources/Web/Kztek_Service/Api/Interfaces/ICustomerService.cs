using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;

namespace Kztek_Service.Api.Interfaces
{
    public interface ICustomerService
    {
        Task<GridModel<MN_CustomerCustomView>> GetPagingByFirst(string key, int pageNumber, int pageSize);

        Task<MN_CustomerDetailCustomView> GetCustomById(string id);
    }
}
