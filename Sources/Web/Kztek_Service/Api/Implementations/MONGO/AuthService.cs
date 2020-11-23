using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Library.Security;
using Kztek_Model.Models;
using Kztek_Service.Api.Interfaces;
using Kztek_Service.OneSignalr.Interfaces;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class AuthService : IAuthService
    {
        private ISY_UserRepository _SY_UserRepository;
        private IOS_PlayerService _OS_PlayerService;

        public AuthService(ISY_UserRepository _SY_UserRepository, IOS_PlayerService _OS_PlayerService)
        {
            this._SY_UserRepository = _SY_UserRepository;
            this._OS_PlayerService = _OS_PlayerService;
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

        public async Task<SY_User> GetById(string id)
        {
            return await _SY_UserRepository.GetOneById(id);
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

                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + model.UserId + "' }");
                query.AppendLine("}");

                result = await _SY_UserRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), user);
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

                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + model.UserId + "' }");
                query.AppendLine("}");

                result = await _SY_UserRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), user);

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

        public async Task<MessageReport> LoginByThirdParty(AuthModel model)
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
                //var pass = CryptoHelper.DecryptPass_User(objUser.Password, objUser.PasswordSalat);

                ////Check mật khẩu
                //if (pass != model.Password)
                //{
                //    result = new MessageReport(false, "Mật khẩu không khớp");
                //    return await Task.FromResult(result);
                //}

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

        public async Task<MessageReport> Register(UserRegister model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra email có tồn tại
                var existed = await GetByUsername(model.Email);
                if (existed != null)
                {
                    result = new MessageReport(false, "Email này đã tồn tại trong hệ thống");
                    return await Task.FromResult(result);
                }

                //Gắn lại giá trị
                var obj = new SY_User()
                {
                    Active = true,
                    Avatar = "",
                    Id = ObjectId.GenerateNewId().ToString(),
                    isAdmin = false,
                    Name = model.Name,
                    Password = model.Password,
                    PasswordSalat = Guid.NewGuid().ToString(),
                    Phone = "",
                    Username = model.Email
                };

                obj.Password = CryptoHelper.EncryptPass_User(obj.Password, obj.PasswordSalat);

                result = await _SY_UserRepository.Add(obj);

            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> CheckEmailExisted(UserForgotModel model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //
                var existed = await GetByUsername(model.Email);
                if (existed == null)
                {
                    result = new MessageReport(false, "Email này không tồn tại trong hệ thống");
                    return await Task.FromResult(result);
                }

                existed.CodeReset = FunctionHelper.GetRandomNumericCharacters(6);

                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + existed.Id + "' }");
                query.AppendLine("}");

                result = await _SY_UserRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), existed);
                if (result.isSuccess)
                {
                    result.Message = "Gửi mã xác nhận vào email";

                    sendEmail(existed.Username, existed.CodeReset);
                }

            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> ForgotPass(UserForgotModel model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra tồn tại
                var existed = await GetByUsername(model.Email);
                if (existed == null)
                {
                    result = new MessageReport(false, "Email này không tồn tại trong hệ thống");
                    return await Task.FromResult(result);
                }

                //Kiểm tra mã khớp
                if (existed.CodeReset != model.Code)
                {
                    result = new MessageReport(false, "Mã xác nhận không khớp");
                    return await Task.FromResult(result);
                }

                //Cập nhật mật khẩu
                existed.CodeReset = "";
                existed.PasswordSalat = Guid.NewGuid().ToString();
                existed.Password = CryptoHelper.EncryptPass_User(model.NewPass, existed.PasswordSalat);

                //Sửa pass

                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + existed.Id + "' }");
                query.AppendLine("}");

                result = await _SY_UserRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), existed);
                if (result.isSuccess)
                {
                    result.Message = "Reset mật khẩu thành công";
                }

            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return await Task.FromResult(result);
        }

        private async Task<bool> sendEmail(string toEmail, string code)
        {
            try
            {
                var e = new SmtpEmailSenderHelper();
                e.EnabledSSL = true;
                e.IsBodyHTML = true;
                e.Password = "pronotdie@#999999";
                e.Port = 587;
                e.SMTPServer = "smtp.gmail.com";
                e.UseDefaultCredential = false;
                e.Username = "maximus93pro@gmail.com";

                var str = new StringBuilder();
                str.AppendLine("<p>Bạn có yêu cầu phục hồi mật khẩu:</p>");
                str.AppendLine(string.Format("<p>Mã phục hồi: {0} </p>", code));

                var result = e.SendSingleMail("Yêu cầu phục hồi mật khẩu", "maximus93pro@gmail.com", toEmail, str.ToString());

                return await Task.FromResult(result);
            }
            catch (Exception ex)
            {
                var k = ex.Message;
                return await Task.FromResult(false);
            }
        }

        public async Task<MessageReport> DeviceRegister(OS_DeviceRegisterModel model)
        {
            var result = new MessageReport(false, "có lỗi xảy ra");

            try
            {
                //Kiểm tra tồn tại
                var existed = await _OS_PlayerService.GetByPlayerId_UserId(model.DeviceId, model.UserId);
                if (existed == null) 
                {
                    existed = new Kztek_Model.Models.OneSignalr.OS_Player() {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = model.UserId,
                        PlayerId = model.DeviceId
                    };

                    result = await _OS_PlayerService.Create(existed);
                }

                result = new MessageReport(true, "Đã đăng ký");
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<MessageReport> DeviceRemove(OS_DeviceRegisterModel model)
        {
            var result = new MessageReport(false, "có lỗi xảy ra");

            try
            {
                //Kiểm tra tồn tại
                var existed = await _OS_PlayerService.GetByPlayerId_UserId(model.DeviceId, model.UserId);
                if (existed != null)
                {
                    result = await _OS_PlayerService.Delete(existed.Id);
                    return result;
                }
                else
                {
                    result = new MessageReport(true, "Đã hủy đăng ký thiết bị");
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<MessageReport> Voip(string fromUsername, string toUsername)
        {
            var result = new MessageReport(false, "error");
            var ids = new List<string>();

            try
            {
                //Lấy người dùng cần gọi
                //var toUser = await GetByUsername(toUsername);
                //if (toUser == null)
                //{
                //    result = new MessageReport(false, "Tài khoản không tồn tại");
                //    return result;
                //}

                ids.Add(toUsername);

                //Lấy danh sách thiết bị người đó
                //var playerIds = await _OS_PlayerService.GetPlayerIdsByUserIds(ids);

                //
                _OS_PlayerService.SendVoip(fromUsername, toUsername, ids);

                result = new MessageReport(true, "Called");

            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }



            return result;
        }
    }
}
