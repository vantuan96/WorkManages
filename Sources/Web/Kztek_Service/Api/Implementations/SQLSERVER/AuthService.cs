using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Library.Security;
using Kztek_Model.Models;
using Kztek_Service.Api.Interfaces;
using Newtonsoft.Json;

namespace Kztek_Service.Api.Implementations.SQLSERVER
{
    public class AuthService : IAuthService
    {
        private ISY_UserRepository _SY_UserRepository;

        public AuthService(ISY_UserRepository _SY_UserRepository)
        {
            this._SY_UserRepository = _SY_UserRepository;
        }

        public async Task<MessageReport> Login(AuthModel model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra username
                var objUser = await GetByUsername(model.Username);
                if (objUser == null)
                {
                    result = new MessageReport(false, "Tài khoản không tồn tại");
                    return await Task.FromResult(result);
                }

                if (objUser.Active == false)
                {
                    result = new MessageReport(false, "Tài khoản bị khóa");
                    return await Task.FromResult(result);
                }

                //Giải mã
                var pass = CryptoHelper.DecryptPass_User(objUser.Password, objUser.PasswordSalat);

                //Check mật khẩu
                if (pass != model.Password)
                {
                    result = new MessageReport(false, "Mật khẩu không khớp");
                    return await Task.FromResult(result);
                }

                //Tạo token
                var token = ApiHelper.GenerateJSON_MobileToken(objUser.Id);

                //Gán lại user
                result = new MessageReport(true, token);
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> CheckValid(AuthModel model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra username
                var objUser = await GetByUsername(model.Username);
                if (objUser == null)
                {
                    result = new MessageReport(false, "Tài khoản không tồn tại");
                    return await Task.FromResult(result);
                }

                if (objUser.Active == false)
                {
                    result = new MessageReport(false, "Tài khoản bị khóa");
                    return await Task.FromResult(result);
                }

                //Giải mã
                var pass = CryptoHelper.DecryptPass_User(objUser.Password, objUser.PasswordSalat);

                //Check mật khẩu
                if (pass != model.Password)
                {
                    result = new MessageReport(false, "Mật khẩu không khớp");
                    return await Task.FromResult(result);
                }

                //Gán lại user
                result = new MessageReport(true, "Hợp lệ");
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        public async Task<SY_User> GetById(string id)
        {
            return await _SY_UserRepository.GetOneById(id);
        }

        private async Task<SY_User> GetByUsername(string username)
        {
            var query = from n in _SY_UserRepository.Table
                        where n.Username == username
                        select n;

            return await Task.FromResult(query.FirstOrDefault());
        }

        public async Task<MessageReport> ResetPass(ResetPassModel model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra tài khoản tồn tại
                var user = await GetById(model.UserId);
                if (user == null)
                {
                    result = new MessageReport(false, "Tài khoản không tồn tại");
                    return await Task.FromResult(result);
                }

                //Giải mã pass
                var depass = CryptoHelper.DecryptPass_User(user.Password, user.PasswordSalat);

                if (depass != model.OldPass)
                {
                    result = new MessageReport(false, "Mật khẩu cũ không khớp");
                    return await Task.FromResult(result);
                }

                //Update pass mới
                user.PasswordSalat = Guid.NewGuid().ToString();
                user.Password = CryptoHelper.EncryptPass_User(model.NewPass, user.PasswordSalat);

                result = await _SY_UserRepository.Update(user);
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> UpdateInfo(UserUpdateModel model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra tài khoản tồn tại
                var user = await GetById(model.UserId);
                if (user == null)
                {
                    result = new MessageReport(false, "Tài khoản không tồn tại");
                    return await Task.FromResult(result);
                }

                //Gán mới:
                user.Name = model.Name;

                //
                if (model.FileUpload != null && model.FileUpload.ContentType != null)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), await AppSettingHelper.GetStringFromAppSetting("FileUpload:UserFolder"));
                    var res = await UploadHelper.UploadFile(model.FileUpload, filePath);

                    if (res.isSuccess == false)
                    {
                        return await Task.FromResult(res);
                    }

                    var userfolder = await AppSettingHelper.GetStringFromAppSetting("FileUpload:UserFolder");
                    user.Avatar = userfolder + user.Id + "/" + model.FileUpload.FileName;
                }

                result = await _SY_UserRepository.Update(user);

                if (result.isSuccess)
                {
                    result.Message = JsonConvert.SerializeObject(user);
                }

                return result;
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        public Task<MessageReport> LoginByThirdParty(AuthModel model)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReport> Register(UserRegister model)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReport> CheckEmailExisted(UserForgotModel model)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReport> ForgotPass(UserForgotModel model)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReport> DeviceRegister(OS_DeviceRegisterModel model)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReport> DeviceRemove(OS_DeviceRegisterModel model)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReport> Voip(string fromUsername, string toUsername)
        {
            throw new NotImplementedException();
        }
    }
}
