using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.MN;

namespace Kztek_Service.Admin.Interfaces.MN
{
    public interface IMN_ContactService
    {
        Task<List<MN_Contact>> GetListByCustomer(string customerid);

        Task<MN_Contact> GetById(string id);

        Task<MessageReport> Create(MN_Contact model);

        Task<MessageReport> Update(MN_Contact model);

        Task<MessageReport> Delete(string id);
    }
}
