using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.MN;

namespace Kztek_Service.Admin.Interfaces.MN
{
    public interface IMN_CustomerGroupService
    {
        Task<List<MN_CustomerGroup>> GetActiveByFirst();

        Task<GridModel<MN_CustomerGroup>> GetPaging(string key, int pageNumber, int pageSize);

        Task<MN_CustomerGroup> GetById(string id);

        Task<MessageReport> Create(MN_CustomerGroup model);

        Task<MessageReport> Update(MN_CustomerGroup model);

        Task<MessageReport> Delete(string id);
    }
}
