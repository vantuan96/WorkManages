using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Security;
using Kztek_Model.Models.MN;
using Kztek_Service.Admin.Interfaces.MN;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Controllers
{
    public class MN_LicenseController : Controller
    {
        private IMN_LicenseService _MN_LicenseService;

        public MN_LicenseController(IMN_LicenseService _MN_LicenseService)
        {
            this._MN_LicenseService = _MN_LicenseService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index(string key, int page = 1, string exportfile = "", string fromdate ="" , string todate ="")
        {
            //if (string.IsNullOrWhiteSpace(fromdate))
            //{
            //    fromdate = DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            //}
            //if (string.IsNullOrWhiteSpace(todate))
            //{
            //    todate = DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            //}

            if (!string.IsNullOrWhiteSpace(exportfile)) {
                await ExportFile(exportfile);
            }
            var gridmodel = await _MN_LicenseService.GetPaging(key, fromdate, todate,page, 10);
            ViewBag.keyValue = key;
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("MN_License", this.HttpContext);

            ViewBag.fromdate = fromdate;
            ViewBag.toodate = !string.IsNullOrWhiteSpace(todate) ? Convert.ToDateTime(todate).ToString("dd/MM/yyyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            return View(gridmodel);
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(MN_License model)
        {
            model = new MN_License();

            return View(await Task.FromResult(model));
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(MN_License model, bool SaveAndCountinue = false)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //
            var existed = await _MN_LicenseService.GetByName(model.ProjectName);
            if (existed != null)
            {
                ModelState.AddModelError("ProjectName", "Tên này đã tồn tại");
                return View(model);
            }

            model.Id = ObjectId.GenerateNewId().ToString();

            //Thực hiện thêm mới
            var result = await _MN_LicenseService.Create(model);
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
            var model = await _MN_LicenseService.GetById(id);

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(MN_License model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _MN_LicenseService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            //
            var existed = await _MN_LicenseService.GetByName(model.ProjectName);
            if (existed != null && existed.Id != model.Id)
            {
                ModelState.AddModelError("ProjectName", "Tên đã tồn tại");
                return View(model);
            }

            oldObj.IsExpire = model.IsExpire;
            oldObj.ProjectName = model.ProjectName;
            oldObj.ExpireDate = model.ExpireDate;

            var result = await _MN_LicenseService.Update(oldObj);
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
            var result = await _MN_LicenseService.Delete(id);

            return Json(result);
        }

        public async Task<int> ExportFile(string id)
        {
            //Lấy dữ liệu
            var obj = await _MN_LicenseService.GetById(id);

            //Mã hóa nội dung
            var content = CryptoHelper.EncryptLicense(JsonConvert.SerializeObject(obj), obj.Id);
            
            byte[] array = Encoding.ASCII.GetBytes(content);

            PrintHelper.Text_Execute(this.HttpContext, array,string.Format("License_{0}", obj.ProjectName));

            return 1;
        }
    }
}
