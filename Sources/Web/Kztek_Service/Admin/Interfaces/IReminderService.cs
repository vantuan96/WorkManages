using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Interfaces
{
    public interface IReminderService
    {
        Task RegisterSchedule(string recordid, string recordtype, string datereminder, string description);

        Task RemoveSchedule(string recordid, string recordtype);
    }
}
