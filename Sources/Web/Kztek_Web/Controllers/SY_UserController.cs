using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kztek_Web.Models;
using Kztek_Model.Models;
using Kztek_Library.Helpers;
using Kztek_Library.Security;
using Kztek.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Kztek_Web.Attributes;
using System.IO;
using OfficeOpenXml;
using System.Globalization;
using Kztek_Library.Models;
using Kztek_Library.Configs;
using OfficeOpenXml.Style;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Web.Controllers
{
    public class SY_UserController : Controller
    {
        private ISY_UserService _SY_UserService;
        private ISY_RoleService _SY_RoleService;
        private IHostingEnvironment _hostingEnvironment;

        public SY_UserController(ISY_UserService _SY_UserService, ISY_RoleService _SY_RoleService, IHostingEnvironment _hostingEnvironment)
        {
            this._SY_UserService = _SY_UserService;
            this._SY_RoleService = _SY_RoleService;
            this._hostingEnvironment = _hostingEnvironment;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index(int page = 1, string export = "0")
        {
            var gridmodel = await _SY_UserService.GetPaging("", page, 10);

            ViewBag.keyValue = "";
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("SY_User", this.HttpContext);

            if (export == "1")
            {
                await ExportFile(this.HttpContext);

                //return View(gridmodel);
            }

            return View(gridmodel);
        }

        // public IActionResult Export()
        // {
        //     ();
        //     return Json("");

        //     // return File(
        //     //     fileContents: data,
        //     //     contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //     //     fileDownloadName: excelName
        //     // );

        //     //return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        // }

        private async Task<bool> ExportFile(HttpContext context)
        {
            //column header
            var Data_ColumnHeader = new List<SelectListModel_Print_Column_Header>();
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "ID" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Họ tên" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Tài khoản" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "MK" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "MKSA" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Là admin" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Hoạt động" });
            Data_ColumnHeader.Add(new SelectListModel_Print_Column_Header { ItemText = "Ảnh" });

            //
            var printConfig = PrintHelper.Template_Excel_V1(PrintConfig.HeaderType.TwoColumns, "Danh sách người dùng", DateTime.Now, SessionCookieHelper.CurrentUser(this.HttpContext).Result, "Kztek", Data_ColumnHeader, 4, 5, 5);

            //
            var gridmodel = await _SY_UserService.GetPaging("", 1, 10);

            return await PrintHelper.Excel_Write<SY_User>(context, gridmodel.Data, "User_" + DateTime.Now.ToString("ddMMyyyyHHmmss"), printConfig);
        }

        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(SY_User_Submit model)
        {
            model = model == null ? new SY_User_Submit() : model;
            model.Data_Role = await _SY_RoleService.GetAllActiveOrder();

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(SY_User_Submit model, bool SaveAndCountinue = false)
        {
            model.Data_Role = await _SY_RoleService.GetAllActiveOrder();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //
            var existed = await _SY_UserService.GetByUsername(model.Username);
            if (existed != null)
            {
                ModelState.AddModelError("Username", "Tài khoản tồn tại");
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = "123456";
            }
            else
            {
                if (model.Password != model.RePassword)
                {
                    ModelState.AddModelError("RePassword", "Mật khẩu không khớp");
                    return View(model);
                }
            }

            var obj = new SY_User()
            {
                Active = model.Active,
                Id = Guid.NewGuid().ToString(),
                Password = model.Password,
                PasswordSalat = Guid.NewGuid().ToString(),
                Name = model.Name,
                Username = model.Username,
                isAdmin = model.isAdmin,
                Phone = model.Phone
            };

            if (!string.IsNullOrWhiteSpace(model.RoleIds))
            {
                var ks = model.RoleIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                model.Roles = new List<string>();
                foreach (var item in ks)
                {
                    model.Roles.Add(item);
                }

                foreach (var item in model.Roles)
                {
                    var t = new SY_Map_User_Role()
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoleId = item,
                        UserId = obj.Id
                    };

                    await _SY_RoleService.CreateMap(t);
                }
            }

            //Mã hóa pass
            obj.Password = CryptoHelper.EncryptPass_User(obj.Password, obj.PasswordSalat);

            //Thực hiện thêm mới
            var result = await _SY_UserService.Create(obj);
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
            var model = await _SY_UserService.GetCustomById(id);
            model.Data_Role = await _SY_RoleService.GetAllActiveOrder();

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(SY_User_Submit model)
        {
            model.Data_Role = await _SY_RoleService.GetAllActiveOrder();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _SY_UserService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            //
            var existed = await _SY_UserService.GetByUsername_notId(model.Username, model.Id);
            if (existed != null)
            {
                ModelState.AddModelError("Username", "Tài khoản tồn tại");
                return View(model);
            }

            oldObj.Active = model.Active;
            oldObj.Name = model.Name;
            oldObj.Username = model.Username;
            oldObj.isAdmin = model.isAdmin;
            oldObj.Phone = model.Phone;

            //Kiểm tra mật khẩu mới
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (model.Password != model.RePassword)
                {
                    ModelState.AddModelError("RePassword", "Mật khẩu không khớp");
                    return View(model);
                }

                //Sinh mã salat mới
                oldObj.PasswordSalat = Guid.NewGuid().ToString();

                //
                oldObj.Password = CryptoHelper.EncryptPass_User(model.Password, oldObj.PasswordSalat);
            }

            //
            await _SY_RoleService.DeleteMap(oldObj.Id);

            if (!string.IsNullOrWhiteSpace(model.RoleIds))
            {
                var ks = model.RoleIds.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                model.Roles = new List<string>();
                foreach (var item in ks)
                {
                    model.Roles.Add(item);
                }

                foreach (var item in model.Roles)
                {
                    var t = new SY_Map_User_Role()
                    {
                        Id = Guid.NewGuid().ToString(),
                        RoleId = item,
                        UserId = oldObj.Id
                    };

                    await _SY_RoleService.CreateMap(t);
                }
            }

            var result = await _SY_UserService.Update(oldObj);
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
            var result = await _SY_UserService.Delete(id);

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> AccountInfo(string id)
        {
            var model = await _SY_UserService.GetCustomById(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AccountInfo(SY_User_Submit model, IFormFile FileAvatar)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _SY_UserService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại");
                return View(model);
            }

            var existedUser = await _SY_UserService.GetByUsername_notId(model.Username, model.Id);
            if (existedUser != null)
            {
                ModelState.AddModelError("", "Tài khoản đã tồn tại");
                return View(model);
            }

            //Có đổi mật khẩu
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                if (string.IsNullOrWhiteSpace(model.OldPassword))
                {
                    ModelState.AddModelError("", "Vui lòng nhập mật khẩu cũ");
                    return View(model);
                }

                if (model.Password != model.RePassword)
                {
                    ModelState.AddModelError("", "Vui lòng nhập lại chính xác mật khẩu mới");
                    return View(model);
                }

                //Tiến hành check mật khẩu cũ
                var oldpass = CryptoHelper.DecryptPass_User(oldObj.Password, oldObj.PasswordSalat);

                if (oldpass != model.OldPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu cũ không chính xác");
                    return View(model);
                }

                //Tạo mk mới
                oldObj.PasswordSalat = Guid.NewGuid().ToString();
                oldObj.Password = CryptoHelper.EncryptPass_User(model.Password, oldObj.PasswordSalat);
            }

            //Có upload file ảnh lên
            if (FileAvatar != null)
            {
                var filePath = _hostingEnvironment.WebRootPath + "/uploads";
                var res = UploadHelper.UploadFile(FileAvatar, filePath).Result;

                if (res.isSuccess == false)
                {
                    return View(model);
                }

                model.Avatar = "/uploads/" + FileAvatar.FileName;
                oldObj.Avatar = "/uploads/" + FileAvatar.FileName;
            }

            oldObj.Username = model.Username;
            oldObj.Name = model.Name;
            oldObj.Phone = model.Phone;

            var result = await _SY_UserService.Update(oldObj);
            if (result.isSuccess)
            {
                TempData["Success"] = "Cập nhật thành công";
                return RedirectToAction("AccountInfo", "SY_User");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }
    }
}
