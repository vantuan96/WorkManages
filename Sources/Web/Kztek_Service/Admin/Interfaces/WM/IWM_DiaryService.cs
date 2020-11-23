using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;

namespace Kztek_Service.Admin.Interfaces.WM
{
    public interface IWM_DiaryService
    {
        Task<List<WM_Diary>> GetAllByUserId_ScheduleId(string userid, string scheduleid);

        Task<List<WM_DiaryCustomViewModel>> GetDiaryByUser(string scheduleid);

        Task<WM_Diary> GetById(string id);

        Task<MessageReport> Create(WM_Diary obj);

        Task<MessageReport> Update(WM_Diary obj);

        Task<MessageReport> Delete(string id);
    }
}