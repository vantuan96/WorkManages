using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.PM;
using Kztek_Model.Models.WM;
using Kztek_Service.Api.Interfaces;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class ReportService : IReportService
    {
        private IPM_WorkRepository _PM_WorkRepository;
        private IWM_TaskUserRepository _WM_TaskUserRepository;

        public ReportService(IPM_WorkRepository _PM_WorkRepository, IWM_TaskUserRepository _WM_TaskUserRepository)
        {
            this._PM_WorkRepository = _PM_WorkRepository;
            this._WM_TaskUserRepository = _WM_TaskUserRepository;
        }

        
        public async Task<Chart_Performance_Personal> ReportByUserId(string userid)
        {
            var model = new Chart_Performance_Personal()
            {
                Project_Total = 0,
                Project_Completed_onTime = 0,
                Project_Completed_notOnTime = 0,
                Project_NotComplete = 0,
                ProjectStatus = new Chart_Performance_Personal_Pie()
                {
                    Doing = 0,
                    Late = 0,
                    OnTime = 0,
                },
                Task_Total = 0,
                Task_Completed_onTime = 0,
                Task_Completed_notOnTime = 0,
                Task_NotComplete = 0,
                TaskStatus = new Chart_Performance_Personal_Pie()
                {
                    Doing = 0,
                    Late = 0,
                    OnTime = 0,
                }
            };
            var time = DateTime.Now;

            //
            var first = new DateTime(time.Year, 1, 1);
            var last = time;

            //Dữ liệu dự án
            var Project_Data = await DataWork_ByUser_ByTime(userid, first, last);

            //Dự liệu công việc hàng ngày
            var Task_Data = await DataTask_ByUser_ByTime(userid, first, last);

            //
            model.Project_Total = Project_Data.Count;
            model.Project_Completed_onTime = Project_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();
            model.Project_Completed_notOnTime = Project_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == false).Count();
            model.Project_NotComplete = Project_Data.Where(n => n.IsCompleted == false).Count();

            model.Task_Total = Task_Data.Count;
            model.Task_Completed_onTime = Task_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();
            model.Task_Completed_notOnTime = Task_Data.Where(n => n.IsCompleted == true && n.IsOnScheduled == false).Count();
            model.Task_NotComplete = Task_Data.Where(n => n.IsCompleted == false).Count();

            //
            model.ProjectStatus.OnTime = (model.Project_Total != 0 && model.Project_Completed_onTime != 0) ? (double) (((double)model.Project_Completed_onTime / (double)model.Project_Total) * 100) : 0;
            model.ProjectStatus.Late = (model.Project_Total != 0 && model.Project_Completed_notOnTime != 0) ? (double)(((double)model.Project_Completed_notOnTime / (double)model.Project_Total) * 100) : 0;
            model.ProjectStatus.Doing = 100 - model.ProjectStatus.OnTime - model.ProjectStatus.Late;

            //
            model.TaskStatus.OnTime = (model.Task_Total != 0 && model.Task_Completed_onTime != 0) ? (double)(((double)model.Task_Completed_onTime / (double)model.Task_Total) * 100) : 0;
            model.TaskStatus.Late = (model.Task_Total != 0 && model.Task_Completed_notOnTime != 0) ? (double)(((double)model.Task_Completed_notOnTime / (double)model.Task_Total) * 100) : 0;
            model.TaskStatus.Doing = 100 - model.TaskStatus.OnTime - model.TaskStatus.Late;

            return model;
        }

        public async Task<Chart_Performance_Grow_Personal> ReportGrowByUserId(string userid)
        {
            var time = DateTime.Now;
            var LastMonth = time.AddMonths(-1);

            var currentFirstDay = new DateTime(time.Year, time.Month, 1);
            var currentLastDay = currentFirstDay.AddMonths(1).AddDays(-1);

            var lastFirstDay = new DateTime(LastMonth.Year, LastMonth.Month, 1);
            var lastLastDay = lastFirstDay.AddMonths(1).AddDays(-1);

            //Current Project, task
            var dataProject = await DataWork_ByUser_ByTime(userid, lastFirstDay, currentLastDay);
            var dataTask = await DataTask_ByUser_ByTime(userid, lastFirstDay, currentLastDay);

            var projectTotal = dataProject.Count;
            var projectCompleteOnTime = dataProject.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();
            var projectPercent = (projectTotal != 0 && projectCompleteOnTime != 0) ? (double)(((double)projectCompleteOnTime / (double)projectTotal) * 100) : 0;

            var taskTotal = dataTask.Count;
            var taskCompleteOnTime = dataTask.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();
            var taskPercent = (taskTotal != 0 && taskCompleteOnTime != 0) ? (double)(((double)taskCompleteOnTime / (double)taskTotal) * 100) : 0;

            //Previous Project, task
            var dataLastMonthProject = await DataWork_ByUser_ByTime(userid, lastFirstDay, lastLastDay);
            var dataLasMonthTask = await DataTask_ByUser_ByTime(userid, lastFirstDay, lastLastDay);

            var lastProjectTotal = dataLastMonthProject.Count;
            var lastProjectCompleteOnTime = dataLastMonthProject.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();
            var lastProjectPercent = (lastProjectTotal != 0 && lastProjectCompleteOnTime != 0) ? (double)(((double)lastProjectCompleteOnTime / (double)lastProjectTotal) * 100) : 0;

            var lastTaskTotal = dataLasMonthTask.Count;
            var lastTaskCompleteOnTime = dataLasMonthTask.Where(n => n.IsCompleted == true && n.IsOnScheduled == true).Count();
            var lastTaskPercent = (lastTaskTotal != 0 && lastTaskCompleteOnTime != 0) ? (double)(((double)lastTaskCompleteOnTime / (double)lastTaskTotal) * 100) : 0;

            //Mapping
            var model = new Chart_Performance_Grow_Personal()
            {
                Project_CurrentPercent = projectPercent,
                Task_CurrentPercent = taskPercent,
                Project_GrowPercent = projectPercent - lastProjectPercent,
                Task_GrowPercent = taskPercent - lastTaskPercent
            };

            return model;
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

       
    }
}
