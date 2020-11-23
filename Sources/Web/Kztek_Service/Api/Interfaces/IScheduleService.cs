using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;

namespace Kztek_Service.Api.Interfaces
{
    public interface IScheduleService
    {
        Task<WM_ScheduleMobile> GetCurrentScheduleByUserId(string userid);

        Task<MessageReport> AddDiary(WM_DiaryMobile model);

        Task<MessageReport> EditDiary(WM_DiaryMobile model);

        Task<MessageReport> DeleteDiary(string id);
    }
}
