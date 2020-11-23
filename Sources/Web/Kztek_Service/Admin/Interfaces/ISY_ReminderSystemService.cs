using Kztek_Core.Models;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Interfaces
{
    public interface ISY_ReminderSystemService
    {
        Task<SY_ReminderSystem> GetById(string id);

        Task<MessageReport> Create(SY_ReminderSystem model);

        Task<MessageReport> Update(SY_ReminderSystem model);

        Task<MessageReport> Delete(string id);

        Task<MessageReport> EditData(string recordid, string recordtype, string datereminder, bool isdone);
    }
}
