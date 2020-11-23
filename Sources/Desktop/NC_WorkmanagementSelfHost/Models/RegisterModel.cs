using Quartz;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NC_WorkmanagementSelfHost.Helpers;
using NC_WorkmanagementSelfHost.Statics;

namespace NC_WorkmanagementSelfHost.Models
{
    public class RegisterModel : IJob
    {
        public string RecordId { get; set; }

        public DateTime DateReminder { get; set; }

        public string RecordType { get; set; } = ""; // 1 - Project, 2 - Task

        public string Message { get; set; } = "";

        public Task Execute(IJobExecutionContext context)
        {
            //ScheduleService.ListenerEvent();
            var model = new ReminderModel()
            {
                RecordId = context.JobDetail.Key.Name,
                RecordType = context.JobDetail.Key.Group,
                Decription = context.JobDetail.Description
            };

            var response = ApiHelper.HttpPost<ReminderModel>(string.Format("{0}api/reminder/receiveschedulereminder", AppSettings.Website), model, "");

            return Task.FromResult(context);
        }
    }
}
