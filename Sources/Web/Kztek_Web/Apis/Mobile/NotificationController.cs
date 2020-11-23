using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Apis.Mobile
{
    [Authorize(Policy = ApiConfig.Auth_Bearer_Mobile)]
    [Route("api/mobile/[controller]")]

    public class NotificationController : Controller
    {
        private INotificationService _NotificationService;

        public NotificationController(INotificationService _NotificationService) {
            this._NotificationService = _NotificationService;
        }

        [HttpGet("getpagingbyfirst")]
        public async Task<GridModel<SY_NotificationCustomView>> getpagingbyfirst(int pageIndex, int pageSize)
        {
            return await _NotificationService.GetPagingByFirst(pageIndex, pageSize);
        }
    }
}