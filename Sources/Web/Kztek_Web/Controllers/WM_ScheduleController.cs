using System;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Model.Models.WM;
using Kztek_Service.Admin.Interfaces.WM;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Kztek_Web.Controllers
{
    public class WM_ScheduleController : Controller
    {
        private IWM_ScheduleService _WM_ScheduleService;

        public WM_ScheduleController(IWM_ScheduleService _WM_ScheduleService)
        {
            this._WM_ScheduleService = _WM_ScheduleService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index(int page = 1, string export = "0")
        {
            var gridmodel = await _WM_ScheduleService.GetPaging("", page, 10);

            ViewBag.keyValue = "";
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("WM_Schedule", this.HttpContext);

            return View(gridmodel);
        }


        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(WM_Schedule_Submit model)
        {
            model = model == null ? new WM_Schedule_Submit() : model;

            return View(await Task.FromResult(model));
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(WM_Schedule_Submit model, bool SaveAndCountinue = false)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var obj = new WM_Schedule()
            {
                DateCreated = DateTime.Now,
                DateEnd = Convert.ToDateTime(model.DateEnd),
                DateStart = Convert.ToDateTime(model.DateStart),
                Description = model.Description,
                Id = ObjectId.GenerateNewId().ToString(),
                Title = model.Title
            };

            //Thực hiện thêm mới
            var result = await _WM_ScheduleService.Create(obj);
            if (result.isSuccess)
            {
                if (SaveAndCountinue)
                {
                    TempData["Success"] = "Thêm mới thành công";
                    return RedirectToAction("Create");
                }

                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(obj);
            }
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var model = await _WM_ScheduleService.GetCustomById(id);

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(WM_Schedule_Submit model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _WM_ScheduleService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            oldObj.DateEnd = Convert.ToDateTime(model.DateEnd);
            oldObj.DateStart = Convert.ToDateTime(model.DateStart);
            oldObj.Description = model.Description;
            oldObj.Title = model.Title;

            var result = await _WM_ScheduleService.Update(oldObj);
            if (result.isSuccess)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _WM_ScheduleService.Delete(id);

            return Json(result);
        }

        public async Task<IActionResult> HomeSchedulePartial()
        {
            DateTime baseDate = DateTime.Now;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);

            var list = await _WM_ScheduleService.GetCurrentWeekSchedule(thisWeekStart, thisWeekEnd);

            return PartialView(list.FirstOrDefault());
        }
    }
}