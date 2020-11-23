using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Controllers
{
    public class MN_CustomerController : Controller
    {
        private IMN_CustomerService _MN_CustomerService;
        private IMN_CustomerGroupService _MN_CustomerGroupService;
        private IMN_ContactService _MN_ContactService;

        public MN_CustomerController(IMN_CustomerService _MN_CustomerService, IMN_CustomerGroupService _MN_CustomerGroupService, IMN_ContactService _MN_ContactService)
        {
            this._MN_CustomerService = _MN_CustomerService;
            this._MN_CustomerGroupService = _MN_CustomerGroupService;
            this._MN_ContactService = _MN_ContactService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index(string key, string customergroupid, int page = 1, string export = "0")
        {
            var gridmodel = await _MN_CustomerService.GetCustomPaging(key, customergroupid, page, 10);

            ViewBag.keyValue = key;
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("MN_Customer", this.HttpContext);

            return View(gridmodel);
        }


        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(MN_Customer model)
        {
            model = model == null ? new MN_Customer() : model;

            ViewBag.Select_CustomerGroup = await Select_CustomerGroup("");

            return View(await Task.FromResult(model));
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(MN_Customer model, bool SaveAndCountinue = false)
        {
            ViewBag.Select_CustomerGroup = await Select_CustomerGroup("");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var obj = new MN_Customer()
            {
                Description = model.Description,
                Id = ObjectId.GenerateNewId().ToString(),
                Name = model.Name,
                CustomerGroupId = model.CustomerGroupId,
            };

            //Thực hiện thêm mới
            var result = await _MN_CustomerService.Create(obj);
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
            var model = await _MN_CustomerService.GetById(id);

            ViewBag.Select_CustomerGroup = await Select_CustomerGroup(model.CustomerGroupId);

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(MN_Customer model)
        {
            ViewBag.Select_CustomerGroup = await Select_CustomerGroup(model.CustomerGroupId);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _MN_CustomerService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            oldObj.Description = model.Description;
            oldObj.Name = model.Name;
            oldObj.CustomerGroupId = model.CustomerGroupId;

            var result = await _MN_CustomerService.Update(oldObj);
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
            var result = await _MN_CustomerService.Delete(id);

            return Json(result);
        }

        public async Task<IActionResult> CustomerContactPartial(string customerid)
        {
            var data = await _MN_ContactService.GetListByCustomer(customerid);

            return PartialView(data);
        }

        public async Task<IActionResult> NewContactPartial(MN_Contact model) 
        {
            return PartialView(model);
        }

        public async Task<IActionResult> ContactEdit(MN_ContactSubmit model)
        {
            var result = new MessageReport(false, "error");

            //Kiểm tra là mới hay cũ cập nhật
            var existed = await _MN_ContactService.GetById(model.Id);
            if (existed == null)
            {
                existed = new MN_Contact()
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    CustomerId = model.CustomerId,
                    ContactType = model.ContactType,
                    Value = model.Value
                };

                result = await _MN_ContactService.Create(existed);
            }
            else
            {
                existed.Value = model.Value;
                existed.ContactType = model.ContactType;

                result = await _MN_ContactService.Update(existed);
            }

            return Json(result);
        }

        public async Task<IActionResult> ContactDelete(string id)
        {
            return Json(await _MN_ContactService.Delete(id));
        }

        public async Task<IActionResult> CreateNewCustomer(MN_Customer model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    result = new MessageReport(false, "Vui lòng điền tên khách hàng");
                    return Json(result);
                }

                //Gán
                model.Id = ObjectId.GenerateNewId().ToString();
                model.Description = "";
                model.Note = "";
                model.CustomerGroupId = "";
                model.DateCreated = DateTime.Now;

                //
                result = await _MN_CustomerService.Create(model);
                if (result.isSuccess)
                {
                    result.Message = model.Id;
                }
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        private async Task<SelectListModel_Chosen> Select_CustomerGroup(string select)
        {
            var data = await _MN_CustomerGroupService.GetActiveByFirst();

            var custom = new List<SelectListModel>();

            foreach (var item in data)
            {
                custom.Add(new SelectListModel()
                {
                    ItemText = item.Name,
                    ItemValue = item.Id
                });
            }

            var model = new SelectListModel_Chosen()
            {
                Data = custom,
                IdSelectList = "CustomerGroupId",
                isMultiSelect = false,
                Placeholder = "",
                Selecteds = !string.IsNullOrWhiteSpace(select) ? select : ""
            };

            return model;
        }


    }
}
