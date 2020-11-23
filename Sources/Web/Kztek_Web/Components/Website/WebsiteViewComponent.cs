using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Components.Breadcrumb
{
    public class WebsiteViewComponent : ViewComponent
    {
        public WebsiteViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = new WebsiteModel()
            {
                WebsiteName = await Kztek_Library.Helpers.AppSettingHelper.GetStringFromAppSetting("WebConfig:WebsiteName")
            };

            return View(model);
        }
    }
}