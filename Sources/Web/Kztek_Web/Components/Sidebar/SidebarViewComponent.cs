using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Admin.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Components.Sidebar
{
    public class SidebarViewComponent : ViewComponent
    {
        private ISY_MenuFunctionService _SY_MenuFunctionService;
        private IHttpContextAccessor HttpContextAccessor;

        public SidebarViewComponent(ISY_MenuFunctionService _SY_MenuFunctionService, IHttpContextAccessor HttpContextAccessor)
        {
            this._SY_MenuFunctionService = _SY_MenuFunctionService;
            this.HttpContextAccessor = HttpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string controllername, string actionname)
        {
            //
            var user = await SessionCookieHelper.CurrentUser(HttpContextAccessor.HttpContext);

            var model = new SidebarModel();
            model.ControllerName = controllername;
            model.ActionName = actionname;

            var data = await _SY_MenuFunctionService.GetAllActiveByUserId(HttpContextAccessor.HttpContext, user);

            model.Data = data.ToList();

            model.CurrentView = model.Data.FirstOrDefault(n => n.ControllerName.Equals(model.ControllerName) && n.ActionName.Equals(model.ActionName));

            model.Breadcrumb = await _SY_MenuFunctionService.GetBreadcrumb(model.CurrentView != null ? model.CurrentView.Id : "", model.CurrentView != null ? model.CurrentView.ParentId : "", "");

            return View(model);
        }
    }
}