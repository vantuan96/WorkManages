using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;

namespace Kztek_Service.Admin.Interfaces.WM
{
    public interface IWM_TaskService
    {
        Task<GridModel<WM_Task>> GetPaging(string key, int pageNumber, int pageSize);

        Task<List<WM_Task>> GetTaskByUserId(string userid);

        Task<List<WM_TaskUser>> GetUserTasksByTaskId(string taskid);

        Task<WM_Task> GetById(string id);

        Task<WM_TaskUser> GetByTaskId_UserId(string taskid, string userid);

        Task<WM_TaskSubmit> GetCustomById(string id);

        Task<WM_TaskSubmit> GetCustomByModel(WM_Task model);

        Task<MessageReport> Create(WM_Task model);

        Task<MessageReport> CreateUserTask(WM_TaskUser model);

        Task<MessageReport> Update(WM_Task model);

        Task<MessageReport> UpdateUserTask(WM_TaskUser model);

        Task<MessageReport> Delete(string id);

        Task<MessageReport> DeleteUserTasksByTaskId(string taskid);
    }
}