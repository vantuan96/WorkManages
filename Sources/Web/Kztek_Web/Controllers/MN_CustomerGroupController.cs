using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Controllers
{
    public class MN_CustomerGroupController : Controller
    {
        private IMN_CustomerGroupService _MN_CustomerGroupService;

        public MN_CustomerGroupController(IMN_CustomerGroupService _MN_CustomerGroupService)
        {
            this._MN_CustomerGroupService = _MN_CustomerGroupService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index(int page = 1, string export = "0")
        {
            var gridmodel = await _MN_CustomerGroupService.GetPaging("", page, 10);

            ViewBag.keyValue = "";
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("MN_CustomerGroup", this.HttpContext);

            return View(gridmodel);
        }


        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(MN_CustomerGroup model)
        {
            model = model == null ? new MN_CustomerGroup() : model;

            return View(await Task.FromResult(model));
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(MN_CustomerGroup model, bool SaveAndCountinue = false)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var obj = new MN_CustomerGroup()
            {
                Description = model.Description,
                Id = ObjectId.GenerateNewId().ToString(),
                Name = model.Name,
                Ordering = model.Ordering,
            };

            //Thực hiện thêm mới
            var result = await _MN_CustomerGroupService.Create(obj);
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
            var model = await _MN_CustomerGroupService.GetById(id);

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(MN_CustomerGroup model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _MN_CustomerGroupService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            oldObj.Description = model.Description;
            oldObj.Name = model.Name;
            oldObj.Ordering = model.Ordering;

            var result = await _MN_CustomerGroupService.Update(oldObj);
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
            var result = await _MN_CustomerGroupService.Delete(id);

            return Json(result);
        }
    }
}
