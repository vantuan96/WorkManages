using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Models;
using Kztek_Service.Admin.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Controllers
{
    public class SY_ReportController : Controller
    {
        private ISY_ReportService _SY_ReportService;
        private ISY_UserService _SY_UserService;

        public SY_ReportController(ISY_ReportService _SY_ReportService, ISY_UserService _SY_UserService)
        {
            this._SY_ReportService = _SY_ReportService;
            this._SY_UserService = _SY_UserService;
        }

        public async Task<IActionResult> Report_PerformanceMonthly(string userids)
        {
            ViewBag.Select_User = await Select_User(userids);

            return View();
        }

        public async Task<IActionResult> Report_PerformanceMonthly_Data(SelectListModel model)
        {
            var ids = JsonConvert.DeserializeObject<List<string>>(model.ItemValue);

            var data = await _SY_ReportService.Users_Performance(ids);

            return Json(data);
        }

        public async Task<IActionResult> Report_PerformanceTeam(string userids)
        {
            ViewBag.Select_User = await Select_User(userids);

            return View();
        }

        public async Task<IActionResult> Report_PerformanceTeam_Data(SelectListModel model)
        {
            var ids = JsonConvert.DeserializeObject<List<string>>(model.ItemValue);

            var data = await _SY_ReportService.Team_Performance(ids);

            return Json(data);
        }


        public async Task<IActionResult> Report_PerformanceGrow(string userids)
        {
            ViewBag.Select_User = await Select_User(userids);

            return View();
        }

        public async Task<IActionResult> Report_PerformanceGrow_Data(SelectListModel model)
        {
            var ids = JsonConvert.DeserializeObject<List<string>>(model.ItemValue);

            var data = await _SY_ReportService.Users_PerformanceGrow(ids);

            return Json(data);
        }








        private async Task<SelectListModel_Chosen> Select_User(string select)
        {
            var model = new SelectListModel_Chosen()
            {
                Data = await Data_User(),
                IdSelectList = "slUser",
                isMultiSelect = true,
                Placeholder = "",
                Selecteds = !string.IsNullOrWhiteSpace(select) ? select : ""
            };

            return model;
        }

        private async Task<List<SelectListModel>> Data_User()
        {
            var cu = new List<SelectListModel>();

            var data = await _SY_UserService.GetAllActive();
            foreach (var item in data)
            {
                cu.Add(new SelectListModel()
                {
                    ItemValue = item.Id,
                    ItemText = item.Username + "(" + item.Name + ")"
                });
            }

            return cu;
        }
    }
}
