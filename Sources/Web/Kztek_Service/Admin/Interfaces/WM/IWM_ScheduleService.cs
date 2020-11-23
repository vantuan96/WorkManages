using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models.WM;

namespace Kztek_Service.Admin.Interfaces.WM
{
    public interface IWM_ScheduleService
    {
        Task<GridModel<WM_Schedule>> GetPaging(string key, int pageNumber, int pageSize);

        Task<List<WM_Schedule>> GetCurrentWeekSchedule(DateTime DateStart, DateTime DateEnd);

        Task<WM_Schedule> GetById(string id);

        Task<WM_Schedule_Submit> GetCustomById(string id);

        Task<WM_Schedule_Submit> GetCustomByModel(WM_Schedule model);

        Task<MessageReport> Create(WM_Schedule model);

        Task<MessageReport> Update(WM_Schedule model);

        Task<MessageReport> Delete(string id);
    }
}