using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NC_WorkmanagementSelfHost.Helpers;
using NC_WorkmanagementSelfHost.Statics;

namespace NC_WorkmanagementSelfHost.Models
{
    public class ScheduleService
    {
        static IScheduler scheduler;

        public async static Task Start()
        {
            scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            await scheduler.Start();
        }

        public async static Task<bool> RemoveJob(RegisterModel model)
        {
            var isStart = await checkScheduleStart();
            if (isStart == false)
            {
                await Start();
            }

            return await scheduler.DeleteJob(new JobKey(model.RecordId, model.RecordType));
        }

        public async static Task Register(RegisterModel model)
        {
            var isStart = await checkScheduleStart();
            if (isStart == false)
            {
                await Start();
            }

            var job = JobBuilder.Create<RegisterModel>()
                .WithIdentity(model.RecordId, model.RecordType)
                .WithDescription(model.Message)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity(model.RecordId, model.RecordType)
                .ForJob(model.RecordId, model.RecordType)
                .StartAt(model.DateReminder)
                .EndAt(model.DateReminder.AddMinutes(15))
                //.WithSimpleSchedule(x => x.WithIntervalInMinutes(2).WithRepeatCount(0))
                .WithDescription(model.Message)
                .Build();

             await scheduler.ScheduleJob(job, trigger);
        }

        public async static Task<bool> checkScheduleStart()
        {
            scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            return scheduler.IsStarted;
        }

        public async static Task Stop()
        {
            if (scheduler.IsStarted)
            {
               await scheduler.Shutdown();
            }
        }

        public async static Task SyncData()
        {
            var response = await ApiHelper.HttpGet(string.Format("{0}api/reminder/data", AppSettings.Website), "");

            if (response.IsSuccessStatusCode)
            {
                var data = await ApiHelper.ConvertResponse<List<SY_ReminderSystem>>(response);

                if (data.Any())
                {
                    foreach (var item in data)
                    {
                        await Register(new RegisterModel()
                        {
                            DateReminder = item.DateReminder,
                            Message = "",
                            RecordId = item.Id,
                            RecordType = item.Type
                        });
                    }
                }
            }
        }
    }
}
