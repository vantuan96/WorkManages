using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;

namespace Kztek_Service.Api.Interfaces
{
    public interface ITaskService
    {
         Task<List<WM_TaskCustomView>> GetCurrentTasksByUserId(string UserId); 

         Task<MessageReport> CompleteTask(WM_TaskComplete model);
    }
}