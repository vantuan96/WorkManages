using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Api.Interfaces
{
    public interface IReminderService
    {
        Task<MessageReport> ReminderToClient(ReminderModel model);

        Task<List<SY_ReminderSystem>> DataNeedReminder();
    }
}
