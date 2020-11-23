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
    public class SY_MenuFunctionController : Controller
    {
        private ISY_MenuFunctionService _SY_MenuFunctionService;
        public SY_MenuFunctionController(ISY_MenuFunctionService _SY_MenuFunctionService)
        {
            this._SY_MenuFunctionService = _SY_MenuFunctionService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index()
        {
            var data = await _SY_MenuFunctionService.GetAll();

            return View(data.ToList());
        }

        // public PartialViewResult SubMenu(List<SY_MenuFunction> listChild, List<SY_MenuFunction> allFunction)
        // {
        //     var model = new SY_MenuFunction_Tree()
        //     {
        //         Data_All = allFunction,
        //         Data_Child = listChild
        //     };

        //     return PartialView(model);
        // }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(SY_MenuFunction_Submit model)
        {

            model = model == null ? new SY_MenuFunction_Submit() : model;
            model.Active = true;

            ViewBag.Data_MenuFunction = await GetMenuList();
            ViewBag.Data_MenuType = StaticList.MenuType();

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(SY_MenuFunction_Submit model, bool SaveAndCountinue = false)
        {
            ViewBag.Data_MenuFunction = await GetMenuList();
            ViewBag.Data_MenuType = StaticList.MenuType();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Gán giá trị
            var id = Guid.NewGuid().ToString();

            model.ControllerName = !string.IsNullOrWhiteSpace(model.ControllerName) ? model.ControllerName.Trim() : string.Format("Controller_{0}", id);
            model.ActionName = !string.IsNullOrWhiteSpace(model.ActionName) ? model.ActionName.Trim() : string.Format("Action_{0}", id);

            var obj = new SY_MenuFunction()
            {
                Id = id,
                MenuName = model.MenuName,
                ControllerName = model.ControllerName,
                ActionName = model.ActionName,
                Icon = model.Icon,
                MenuType = model.MenuType,
                ParentId = string.IsNullOrWhiteSpace(model.ParentId) ? "" : model.ParentId,
                Active = model.Active,
                SortOrder = model.SortOrder,
                DateCreated = DateTime.Now,
            };

            //Thực hiện thêm mới
            var result = await _SY_MenuFunctionService.Create(obj);
            if (result.isSuccess)
            {
                if (SaveAndCountinue)
                {
                    TempData["Success"] = "Thêm mới thành công";
                    return RedirectToAction("Create", new { ControllerName = obj.ControllerName, ParentId = obj.ParentId, MenuType = obj.MenuType });
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
            var model = await _SY_MenuFunctionService.GetCustomById(id);

            ViewBag.Data_MenuFunction = await GetMenuList();
            ViewBag.Data_MenuType = StaticList.MenuType();

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(SY_MenuFunction_Submit model)
        {
            ViewBag.Data_MenuFunction = await GetMenuList();
            ViewBag.Data_MenuType = StaticList.MenuType();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _SY_MenuFunctionService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            model.ControllerName = !string.IsNullOrWhiteSpace(model.ControllerName) ? model.ControllerName.Trim() : string.Format("Controller_{0}", model.Id);
            model.ActionName = !string.IsNullOrWhiteSpace(model.ActionName) ? model.ActionName.Trim() : string.Format("Action_{0}", model.Id);

            oldObj.MenuName = model.MenuName;
            oldObj.ControllerName = model.ControllerName.Trim();
            oldObj.ActionName = model.ActionName.Trim();
            oldObj.ParentId = string.IsNullOrWhiteSpace(model.ParentId) ? "" : model.ParentId;
            oldObj.Active = model.Active;
            oldObj.Icon = model.Icon;
            oldObj.MenuType = model.MenuType;
            oldObj.SortOrder = model.SortOrder;

            var result = await _SY_MenuFunctionService.Update(oldObj);
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
            var result = await _SY_MenuFunctionService.Delete(id);

            return Json(result);
        }

        private async Task<List<SY_MenuFunction_Submit>> GetMenuList()
        {
            var list = new List<SY_MenuFunction_Submit>
            {
                new SY_MenuFunction_Submit {  Id = "", MenuName = "- Chọn danh mục -" }
            };

            var MenuList = await _SY_MenuFunctionService.GetAllCustomActiveOrder();
            var parent = MenuList.Where(c => c.ParentId == "").ToList();
            if (parent.Any())
            {
                foreach (var item in parent)
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    list.Add(new SY_MenuFunction_Submit { Id = item.Id, MenuName = item.MenuName });

                    var listChild = MenuList.Where(c => c.ParentId == item.Id).ToList();

                    //Gọi action để lấy danh sách submenu theo id
                    if (listChild.Any())
                    {
                        Children(listChild, MenuList, list, item);
                    }

                    list.Add(new SY_MenuFunction_Submit { Id = "", MenuName = "-----" });

                }
            }
            return list;
        }

        private void Children(List<SY_MenuFunction_Submit> listChild, List<SY_MenuFunction_Submit> allFunction, List<SY_MenuFunction_Submit> lst, SY_MenuFunction_Submit itemParent)
        {
            //Kiểm tra có dữ liệu chưa
            if (listChild.Any())
            {
                foreach (var item in listChild)
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    lst.Add(new SY_MenuFunction_Submit { Id = item.Id, MenuName = itemParent.MenuName + " \\ " + item.MenuName });

                    //Gọi action để lấy danh sách submenu theo id
                    var child = allFunction.Where(c => c.ParentId == item.Id).ToList();

                    //Gọi action để lấy danh sách submenu theo id
                    if (child.Any())
                    {
                        item.MenuName = itemParent.MenuName + " \\ " + item.MenuName;
                        Children(child, allFunction, lst, item);
                    }
                }
            }
        }
    }
}
