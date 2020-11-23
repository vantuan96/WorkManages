using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Models;

namespace Kztek_Service.Api.Interfaces
{
    public interface INotificationService
    {
        Task<GridModel<SY_NotificationCustomView>> GetPagingByFirst(int pageNumber, int pageSize);
    }
}