using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Admin.Interfaces;
using Kztek_Service.Admin.Interfaces.WM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Components.Breadcrumb
{
    public class DiaryViewComponent : ViewComponent
    {
        private IWM_DiaryService _WM_DiaryService;
        public DiaryViewComponent(IWM_DiaryService _WM_DiaryService)
        {
            this._WM_DiaryService = _WM_DiaryService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string scheduleid)
        {
            var data = await _WM_DiaryService.GetDiaryByUser(scheduleid);

            return View(data);
        }
    }
}