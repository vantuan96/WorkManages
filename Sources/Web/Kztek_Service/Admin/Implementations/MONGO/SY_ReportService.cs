using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Model.Models.PM;
using Kztek_Model.Models.WM;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Service.Admin.Implementations.MONGO
{
    public class SY_ReportService : ISY_ReportService
    {
        private ISY_UserRepository _SY_UserRepository;
        private IPM_WorkRepository _PM_WorkRepository;
        private IWM_TaskUserRepository _WM_TaskUserRepository;

        public SY_ReportService(ISY_UserRepository _SY_UserRepository, IPM_WorkRepository _PM_WorkRepository, IWM_TaskUserRepository _WM_TaskUserRepository)
        {
            this._SY_UserRepository = _SY_UserRepository;
            this._PM_WorkRepository = _PM_WorkRepository;
            this._WM_TaskUserRepository = _WM_TaskUserRepository;
        }

        public async Task<List<Chart_Performance_Monthly>> Users_Performance(List<string> userids)
        {
            var time = DateTime.Now;

            var data = new List<Chart_Performance_Monthly>();

            //Lấy danh sách người dùng
            var users = await GetUsersByIds(userids);

            foreach (var item in users)
            {
                data.Add(new Chart_Performance_Monthly()
                {
                    UserId = item.Id,
                    Username = item.Username,
                    Details = await DetailByMonth_User(item.Id, time)
                });
            }

            return data;
        }

        public async Task<List<Chart_Performance_Team>> Team_Performance(List<string> userids)
        {
            var data = new List<Chart_Performance_Team>();

            var time = DateTime.Now;

            //Lấy danh sách người dùng
            var users = await GetUsersByIds(userids);

            foreach (var item in users)
            {
                data.Add(new Chart_Performance_Team()
                {
                    UserId = item.Id,
                    Username = item.Username,
                    Percent = await CurrentPercent_User(item.Id, time)
                });
            }

            return data;
        }

        public async Task<List<Chart_Performance_Grow>> Users_PerformanceGrow(List<string> userids)
        {
            var data = new List<Chart_Performance_Grow>();

            var time = DateTime.Now;

            //Lấy danh sách người dùng
            var users = await GetUsersByIds(userids);

            foreach (var item in users)
            {
                data.Add(new Chart_Performance_Grow()
                {
                    UserId = item.Id,
                    Username = item.Username,
                    Details = await Grow_Detail(item.Id, time)
                });
            }

            return data;
        }



        private async Task<List<SY_User>> GetUsersByIds(List<string> userids)
        {
            var count = 0;

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'_id': { '$in': [");

            foreach (var item in userids)
            {
                count++;
                query.AppendLine(string.Format("'{0}'{1}", item, count == userids.Count ? "" : ","));
            }

            query.AppendLine("]}");

            query.AppendLine("}");

            return await _SY_UserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        private async Task<List<Chart_Performance_Monthly_Detail>> DetailByMonth_User(string userid, DateTime now)
        {
            var data = new List<Chart_Performance_Monthly_Detail>();

            for (int i = 1; i <= now.Month + 1; i++)
            {
                var first = new DateTime(now.Year, i, 1);
                var last = first.AddMonths(1).AddDays(-1);

                //Dữ liệu dự án
                var Project_Data = await DataWork_ByUser_ByTime(userid, first, last);

                //Dự liệu công việc hàng ngày
                var Task_Data = await DataTask_ByUser_ByTime(userid, first, last);

                var k = new Chart_Performance_Monthly_Detail()
                {
                    Month = i.ToString(),
                    Project_Total = Project_Data.Count,
                    Project_Completed = Project_Data.Where(n => n.IsCompleted == true).Count(),
                    Project_Completed_onTime = Project_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count(),
                    Project_Completed_notOnTime = Project_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == false).Count(),
                    Task_Total = Task_Data.Count,
                    Task_Completed = Task_Data.Where(n => n.IsCompleted == true).Count(),
                    Task_Completed_onTime = Task_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count(),
                    Task_Completed_notOnTime = Task_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == false).Count(),
                };

                data.Add(k);
            }

            return data;
        }

        private async Task<List<PM_Work>> DataWork_ByUser_ByTime(string userid, DateTime fromdate, DateTime todate)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'UserId': { '$eq' : '" + userid + "' }");

            query.AppendLine(", 'DateRealStart': {");

            query.AppendLine(" '$gte': ISODate('" + fromdate.ToString("yyyy-MM-dd") + "T00:00:00.000+07:00') ");
            query.AppendLine(", '$lt': ISODate('" + todate.AddDays(1).ToString("yyyy-MM-dd") + "T00:00:00.000+07:00') ");

            query.AppendLine("}");

            query.AppendLine("}");

            return await _PM_WorkRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        private async Task<List<WM_TaskUser>> DataTask_ByUser_ByTime(string userid, DateTime fromdate, DateTime todate)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'UserId': { '$eq' : '" + userid + "' }");

            query.AppendLine(", 'DateCreated': {");

            query.AppendLine(" '$gte': ISODate('" + fromdate.ToString("yyyy-MM-dd") + "T00:00:00.000+07:00') ");
            query.AppendLine(", '$lt': ISODate('" + todate.AddDays(1).ToString("yyyy-MM-dd") + "T00:00:00.000+07:00') ");

            query.AppendLine("}");

            query.AppendLine("}");

            return await _WM_TaskUserRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        private async Task<Chart_Performance_Team_Detail> CurrentPercent_User(string userid, DateTime now)
        {
            var first = new DateTime(now.Year, 1, 1);
            var last = now;

            //Dữ liệu dự án
            var Project_Data = await DataWork_ByUser_ByTime(userid, first, last);

            //Dự liệu công việc hàng ngày
            var Task_Data = await DataTask_ByUser_ByTime(userid, first, last);

            //% của dự án
            var Project_Total = Project_Data.Count;
            var Project_Completed_onTime = Project_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();

            //% của công việc
            var Task_Total = Task_Data.Count;
            var Task_Completed_onTime = Task_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();

            var model = new Chart_Performance_Team_Detail()
            {
                Project_Percent = (Project_Total != 0 && Project_Completed_onTime != 0) ? (double)(((double)Project_Completed_onTime / (double)Project_Total) * 100) : 0,
                Task_Percent = (Task_Total != 0 && Task_Completed_onTime != 0) ? (double)(((double)Task_Completed_onTime / (double)Task_Total) * 100) : 0,
            };

            return model;
        }

        private async Task<List<Chart_Performance_Grow_Detail>> Grow_Detail(string userid, DateTime now)
        {
            var data = new List<Chart_Performance_Grow_Detail>();

            for (int i = 0; i <= now.Month; i++)
            {
                var first = DateTime.Now;
                var last = DateTime.Now;
                
                if (i == 0)
                {
                    first = new DateTime(now.Year - 1, 12, 1);
                    last = first.AddMonths(1).AddDays(-1);
                } else
                {
                    first = new DateTime(now.Year, i, 1);
                    last = first.AddMonths(1).AddDays(-1);
                }

                //Dữ liệu dự án
                var Project_Data = await DataWork_ByUser_ByTime(userid, first, last);

                //% của dự án
                var Project_Total = Project_Data.Count;
                var Project_Completed_onTime = Project_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();

                //Dự liệu công việc hàng ngày
                var Task_Data = await DataTask_ByUser_ByTime(userid, first, last);

                //% của công việc
                var Task_Total = Task_Data.Count;
                var Task_Completed_onTime = Task_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();

                var k = new Chart_Performance_Grow_Detail()
                {
                    Month = i.ToString(),
                    Project_CurrentPercent = (Project_Total != 0 && Project_Completed_onTime != 0) ? (double)(((double)Project_Completed_onTime / (double)Project_Total) * 100) : 0,
                    Task_CurrentPercent = (Task_Total != 0 && Task_Completed_onTime != 0) ? (double)(((double)Task_Completed_onTime / (double)Task_Total) * 100) : 0
                };

                data.Add(k);
            }

            for (int i = 0; i < data.Count; i++)
            {
                if (i == 0)
                {
                    data[i].Project_GrowPercent = 0;
                    data[i].Task_GrowPercent = 0;
                } else
                {
                    data[i].Project_GrowPercent = data[i].Project_CurrentPercent - data[i - 1].Project_CurrentPercent;
                    data[i].Task_GrowPercent = data[i].Task_CurrentPercent - data[i - 1].Task_CurrentPercent;
                }
            }

            return data;
        }
    }
}
