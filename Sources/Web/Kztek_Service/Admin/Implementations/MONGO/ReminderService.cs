using Kztek_Library.Helpers;
using Kztek_Service.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Implementations.MONGO
{
    public class ReminderService : IReminderService
    {
        public ReminderService()
        {

        }

        public async Task RegisterSchedule(string recordid, string recordtype, string datereminder, string description)
        {
            var paramPs = new Dictionary<string, string>();
            paramPs.Add("RecordId", recordid);
            paramPs.Add("DateReminder", datereminder);
            paramPs.Add("RecordType", recordtype);
            paramPs.Add("Message", description);

            var respond = await ApiHelper.HttpPostFormData(string.Format("{0}api/reminder/registerreminder", await AppSettingHelper.GetStringFromAppSetting("SelfHost")), paramPs, "");
        }

        public async Task RemoveSchedule(string recordid, string recordtype)
        {
            var paramPs = new Dictionary<string, string>();
            paramPs.Add("RecordId", recordid);
            paramPs.Add("RecordType", recordtype);

            var respond = await ApiHelper.HttpPostFormData(string.Format("{0}api/reminder/removeregisterreminder", await AppSettingHelper.GetStringFromAppSetting("SelfHost")), paramPs, "");
        }
    }
}
