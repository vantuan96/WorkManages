using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;
using Kztek_Service.OneSignalr.Interfaces;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Controllers
{
    public class SY_NotificationController : Controller
    {
        private ISY_NotificationService _SY_NotificationService;
        private IOS_PlayerService _OS_PlayerService;

        public SY_NotificationController(ISY_NotificationService _SY_NotificationService, IOS_PlayerService _OS_PlayerService)
        {
            this._SY_NotificationService = _SY_NotificationService;
            this._OS_PlayerService = _OS_PlayerService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index()
        {
            var list = await _SY_NotificationService.GetAll();

            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("SY_Notification", this.HttpContext);

            return View(list);
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(SY_Notification model)
        {
            model = model == null ? new SY_Notification() : model;

            return View(await Task.FromResult(model));
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(SY_Notification model, bool SaveAndCountinue = false)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Id = ObjectId.GenerateNewId().ToString();
            model.DateCreated = DateTime.Now;

            //Thực hiện thêm mới
            var result = await _SY_NotificationService.Create(model);
            if (result.isSuccess)
            {
                SendNotification(model);

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
                return View(model);
            }
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var model = await _SY_NotificationService.GetById(id);

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(SY_Notification model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _SY_NotificationService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            oldObj.Title = model.Title;
            oldObj.Description = model.Description;

            var result = await _SY_NotificationService.Update(oldObj);
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
            var result = await _SY_NotificationService.Delete(id);

            return Json(result);
        }

        private async Task<MessageReport> SendNotification(SY_Notification model)
        {
            var obj = new OneSignalrMessage()
            {
                Description = model.Description,
                Id = model.Id,
                PlayerIds = new string[] {},
                Title = "Thông báo: " + model.Title,
                UserIds = "",
                View = OneSignalConfig.NotificationPage
            };

            return await _OS_PlayerService.SendNotification(obj);
        }

        public async Task<IActionResult> HomeNotification() {
            var list = await _SY_NotificationService.GetAllOrder();

            return PartialView(list);
        }
    }
}
