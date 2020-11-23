using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kztek_Web.Models;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Library.Configs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Kztek_Library.Security;
using Kztek_Library.Helpers;
using Kztek_Library.Functions;
using Kztek.Security;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Web.Controllers
{
    public class LoginController : Controller
    {
        private ISY_UserService _SY_UserService;

        public LoginController(ISY_UserService _SY_UserService)
        {
            this._SY_UserService = _SY_UserService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new AuthModel();
            model.isAny = _SY_UserService.GetAll().Result.Any();

            return View(model);
        }

        [HttpPost]
        public IActionResult Index(AuthModel model)
        {
            model.isAny = _SY_UserService.GetAll().Result.Any();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var objUser = new SY_User();
            var result = _SY_UserService.Login(model, out objUser).Result;

            if (result.isSuccess)
            {
                Session_Cookie(model, objUser);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }
        // private void CacheMenuFunctionByUser(SessionModel model)
        // {

        //     AuthHelper.MenuFunctionByUserId(model);
        // }

        private void Session_Cookie(AuthModel model, SY_User user)
        {
            var host = Request.Host.Host;

            //Session
            var userSession = new SessionModel
            {
                UserId = user.Id,
                Name = user.Name,
                Username = user.Username,
                Avatar = user.Avatar,
                isAdmin = user.isAdmin
            };


            //CacheMenuFunctionByUser(userSession);

            //Mã hóa
            var serializeModel = JsonConvert.SerializeObject(userSession);
            var encryptModel = CryptoHelper.EncryptSessionCookie_User(serializeModel);

            //Lưu lại trong session
            HttpContext.Session.SetString(SessionConfig.Kz_UserSession, encryptModel);

            //Kiểm tra có lưu cookie
            if (model.isRemember)
            {
                var option = new CookieOptions();
                option.Expires = DateTime.Now.AddMonths(1);
                HttpContext.Response.Cookies.Append(CookieConfig.Kz_UserCookie, encryptModel);
            }
        }

        public IActionResult Logout(string userid)
        {
            HttpContext.Session.Remove(SessionConfig.Kz_UserSession);
            HttpContext.Response.Cookies.Delete(CookieConfig.Kz_UserCookie);

            CacheFunction.Clear(this.HttpContext, string.Format(CacheConfig.Kz_User_MenuFunctionCache_Key, userid, SecurityModel.Cache_Key));

            return RedirectToAction("Index", "Login");
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterModel();
            model.isAny = _SY_UserService.GetAll().Result.Any();
            if (model.isAny)
            {
                return RedirectToAction("Index", "Login");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            model.isAny = _SY_UserService.GetAll().Result.Any();
            if (model.isAny)
            {
                return RedirectToAction("Index", "Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Kiểm tra mật khẩu
            if (model.Password != model.RePassword)
            {
                ModelState.AddModelError("", "Mật khẩu không khớp");
                return View(model);
            }

            var salat = Guid.NewGuid().ToString();

            var obj = new SY_User()
            {
                Id = Guid.NewGuid().ToString(),
                Name = model.Name,
                Username = model.Username,
                Active = true,
                Avatar = "",
                Password = CryptoHelper.EncryptPass_User(model.Password, salat),
                PasswordSalat = salat,
                isAdmin = true
            };

            var result = _SY_UserService.Create(obj).Result;

            if (result.isSuccess)
            {
                return RedirectToAction("Index", "Login");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
                return View(model);
            }
        }
    }
}
