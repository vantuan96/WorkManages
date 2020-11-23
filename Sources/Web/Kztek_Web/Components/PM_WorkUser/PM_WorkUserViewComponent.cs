using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Admin;
using Kztek_Service.Admin.Interfaces;
using Kztek_Service.Admin.Interfaces.PM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Components.ChosenSelect
{
    public class PM_WorkUserViewComponent : ViewComponent
    {
        private IHttpContextAccessor HttpContextAccessor;
        private IPM_WorkService _PM_WorkService;
        private ISY_UserService _SY_UserService;

        public PM_WorkUserViewComponent(IHttpContextAccessor HttpContextAccessor, IPM_WorkService _PM_WorkService, ISY_UserService _SY_UserService)
        {
            this.HttpContextAccessor = HttpContextAccessor;
            this._PM_WorkService = _PM_WorkService;
            this._SY_UserService = _SY_UserService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string componentid)
        {
            var data = await _PM_WorkService.GetAllByComponentId(componentid);

            var users = await _SY_UserService.GetAllByIds(data.Select(n => n.UserId).ToList());

            var custom = new PM_WorkUserModel()
            {
                Data_User = users,
                Data_Work = data
            };

            return View(custom);
        }
    }
}