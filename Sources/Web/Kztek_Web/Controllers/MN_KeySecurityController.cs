using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Security;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Controllers
{
    public class MN_KeySecurityController : Controller
    {
        private IMN_KeySecurityService _MN_KeySecurityService;
        private IMN_KeyCardService _MN_KeyCardService;

        public MN_KeySecurityController(IMN_KeySecurityService _MN_KeySecurityService, IMN_KeyCardService _MN_KeyCardService)
        {
            this._MN_KeySecurityService = _MN_KeySecurityService;
            this._MN_KeyCardService = _MN_KeyCardService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index(string key, int page = 1)
        {
            var gridmodel = await _MN_KeySecurityService.GetPaging(key, page, 10);

            ViewBag.keyValue = key;
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("MN_KeySecurity", this.HttpContext);

            return View(gridmodel);
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(MN_KeySecurity model)
        {
            model = new MN_KeySecurity();
            model.Code = FunctionHelper.GetRandomNumericCharacters(8);
            model.KeyA = FunctionHelper.GetRandomCharacters(12);
            model.KeyB = FunctionHelper.GetRandomCharacters(12);

            return View(await Task.FromResult(model));
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(MN_KeySecurity model, bool SaveAndCountinue = false)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //
            var existed = await _MN_KeySecurityService.GetByCode(model.Code);
            if (existed != null)
            {
                ModelState.AddModelError("Code", "Tên này đã tồn tại");
                return View(model);
            }

            if (model.KeyA == model.KeyB)
            {
                ModelState.AddModelError("", "2 key phải khác nhau");
                return View(model);
            }

            //check trùng key với dự án khác
            var checkExitKeyA = await _MN_KeySecurityService.GetByKeyA(model.KeyA);
            if (checkExitKeyA != null)
            {
                ModelState.AddModelError("KeyA", "Key A đã tồn tại");
                return View(model);
            }

            var checkExitKeyB = await _MN_KeySecurityService.GetByKeyB(model.KeyB);
            if (checkExitKeyB != null)
            {
                ModelState.AddModelError("KeyB", "Key B đã tồn tại");
                return View(model);
            }

            //Gán giá trị
            model.Id = ObjectId.GenerateNewId().ToString();
            
            model.IsDeleted = false;
            model.KeyA = CryptoHelper.EncryptKey(model.KeyA, model.Id);
            model.KeyB = CryptoHelper.EncryptKey(model.KeyB, model.Id);

            //Thực hiện thêm mới
            var result = await _MN_KeySecurityService.Create(model);
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
                return View(model);
            }
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var model = await _MN_KeySecurityService.GetById(id);
            model.KeyA = CryptoHelper.DecryptKey(model.KeyA, model.Id);
            model.KeyB = CryptoHelper.DecryptKey(model.KeyB, model.Id);

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(MN_KeySecurity model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _MN_KeySecurityService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            //
            var existed = await _MN_KeySecurityService.GetByCode(model.Code);
            if (existed != null && existed.Id != model.Id)
            {
                ModelState.AddModelError("ProjectName", "Tên đã tồn tại");
                return View(model);
            }

            oldObj.Name = model.Name;
            oldObj.Code = model.Code;
            oldObj.Description = model.Description;
            oldObj.Note = model.Note;

            var result = await _MN_KeySecurityService.Update(oldObj);
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
            var result = await _MN_KeySecurityService.Delete(id);

            return Json(result);
        }

        public async Task<IActionResult> GetCode()
        {
            string key = FunctionHelper.GetRandomNumericCharacters(8);
            return Json(await Task.FromResult(key));
        }

        public async Task<IActionResult> GetKey()
        {
            string key = FunctionHelper.GetRandomCharacters(12);
            return Json(await Task.FromResult(key));
        }

        public async Task<IActionResult> CardPartial(string key, string keysecurityid, string fromdate, string todate, int pageindex)
        {
            var data = await _MN_KeyCardService.GetPaging(key, keysecurityid, fromdate, todate, pageindex, 10);

            return Json(data);
        }
    }
}
