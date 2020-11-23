using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kztek_Web.Models;
using Kztek_Model.Models;
using Kztek_Library.Helpers;
using Kztek_Web.Attributes;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Web.Controllers
{
    public class SY_RoleController : Controller
    {
        private ISY_RoleService _SY_RoleService;
        private ISY_MenuFunctionService _SY_MenuFunctionService;

        public SY_RoleController(ISY_RoleService _SY_RoleService, ISY_MenuFunctionService _SY_MenuFunctionService)
        {
            this._SY_RoleService = _SY_RoleService;
            this._SY_MenuFunctionService = _SY_MenuFunctionService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index()
        {
            var list = await _SY_RoleService.GetAll();

            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("SY_Role", this.HttpContext);

            return View(list.ToList());
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(SY_Role_Submit model)
        {
            model = model == null ? new SY_Role_Submit() : model;
            model.Data_Tree = await _SY_MenuFunctionService.GetAllActive();

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(SY_Role_Submit model, bool SaveAndCountinue = false)
        {
            model.Data_Tree = await _SY_MenuFunctionService.GetAllActive();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var obj = new SY_Role()
            {
                RoleName = model.RoleName,
                Description = model.Description,
                Active = model.Active,
                Id = Guid.NewGuid().ToString()
            };

            if (!string.IsNullOrWhiteSpace(model.MenuFunctionIds))
            {
                var ks = model.MenuFunctionIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                model.MenuFunctions = new List<string>();
                foreach (var item in ks)
                {
                    model.MenuFunctions.Add(item);
                }

                foreach (var item in model.MenuFunctions)
                {
                    var t = new SY_Map_Role_Menu()
                    {
                        Id = Guid.NewGuid().ToString(),
                        MenuId = item,
                        RoleId = obj.Id
                    };

                    await _SY_MenuFunctionService.CreateMap(t);
                }
            }

            //Thực hiện thêm mới
            var result = await _SY_RoleService.Create(obj);
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
            var model = await _SY_RoleService.GetCustomById(id);
            model.Data_Tree = await _SY_MenuFunctionService.GetAllActive();

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(SY_Role_Submit model)
        {
            model.Data_Tree = await _SY_MenuFunctionService.GetAllActive();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _SY_RoleService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            oldObj.Active = model.Active;
            oldObj.Description = model.Description;
            oldObj.RoleName = model.RoleName;

            //Cập nhật lại menu
            await _SY_MenuFunctionService.DeleteMap(oldObj.Id);

            if (!string.IsNullOrWhiteSpace(model.MenuFunctionIds))
            {
                var ks = model.MenuFunctionIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                model.MenuFunctions = new List<string>();
                foreach (var item in ks)
                {
                    model.MenuFunctions.Add(item);
                }

                foreach (var item in model.MenuFunctions)
                {
                    var t = new SY_Map_Role_Menu()
                    {
                        Id = Guid.NewGuid().ToString(),
                        MenuId = item,
                        RoleId = oldObj.Id
                    };

                    await _SY_MenuFunctionService.CreateMap(t);
                }
            }

            var result = await _SY_RoleService.Update(oldObj);
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
            var result = await _SY_RoleService.Delete(id);

            return Json(result);
        }
    }
}
