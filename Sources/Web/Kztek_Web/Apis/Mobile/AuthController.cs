using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Apis.Mobile
{
    [Authorize(Policy = ApiConfig.Auth_Bearer_Mobile)]
    [Route("api/mobile/[controller]")]
    public class AuthController : Controller
    {
        private IAuthService _AuthService;

        public AuthController(IAuthService _AuthService)
        {
            this._AuthService = _AuthService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<MessageReport> login([FromBody]AuthModel model)
        {
            return await _AuthService.Login(model);
        }

        [AllowAnonymous]
        [HttpPost("loginbythirdparty")]
        public async Task<MessageReport> loginbythirdparty([FromBody]AuthModel model)
        {
            return await _AuthService.LoginByThirdParty(model);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<SY_User> getbyid(string id)
        {
            return await _AuthService.GetById(id);
        }

        [HttpPost("checkvalid")]
        public async Task<MessageReport> checkvalid([FromBody] AuthModel model)
        {
            return await _AuthService.CheckValid(model);
        }

        [HttpPost("reset")]
        public async Task<MessageReport> reset([FromBody] ResetPassModel model)
        {
            return await _AuthService.ResetPass(model);
        }

        [HttpPost("updateinfo")]
        public async Task<MessageReport> updateinfo([FromForm] UserUpdateModel model)
        {
            return await _AuthService.UpdateInfo(model);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<MessageReport> register([FromBody] UserRegister model)
        {
            return await _AuthService.Register(model);
        }

        [AllowAnonymous]
        [HttpPost("sendrequestchangepassword")]
        public async Task<MessageReport> sendRequestChangePassword([FromBody] UserForgotModel model)
        {
            return await _AuthService.CheckEmailExisted(model);
        }

        [AllowAnonymous]
        [HttpPost("changepassword")]
        public async Task<MessageReport> changePassword([FromBody] UserForgotModel model)
        {
            return await _AuthService.ForgotPass(model);
        }

        [HttpPost("deviceregister")]
        public async Task<MessageReport> deviceregister([FromBody] OS_DeviceRegisterModel model)
        {
            return await _AuthService.DeviceRegister(model);
        }

        [HttpPost("deviceremove")]
        public async Task<MessageReport> deviceremove([FromBody] OS_DeviceRegisterModel model)
        {
            return await _AuthService.DeviceRemove(model);
        }

        [HttpPost("call")]
        public async Task<MessageReport> call([FromBody] OS_CallModel model)
        {
            return await _AuthService.Voip(model.FromUser, model.ToUser);
        }
    }
}
