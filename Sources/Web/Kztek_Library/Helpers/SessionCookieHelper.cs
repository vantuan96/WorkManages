using System.Threading.Tasks;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Kztek_Library.Security;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Kztek_Library.Helpers
{
    public class SessionCookieHelper
    {
        public static Task<SessionModel> CurrentUser(HttpContext HttpContext)
        {
            var model = new SessionModel();

            //Lấy session
            var sessionValue = HttpContext.Session.GetString(SessionConfig.Kz_UserSession);

            //Kiểm tra tồn tại => chuyển sang lấy cookie
            if (string.IsNullOrWhiteSpace(sessionValue))
            {
                //Kiểm tra cookie
                var cookieValue = HttpContext.Request.Cookies[CookieConfig.Kz_UserCookie];

                if (string.IsNullOrWhiteSpace(cookieValue))
                {
                    model = null;
                }
                else
                {
                    //Giải mã
                    var decryptModel = CryptoHelper.DecryptSessionCookie_User(cookieValue);

                    if (!string.IsNullOrWhiteSpace(decryptModel))
                    {
                        model = JsonConvert.DeserializeObject<SessionModel>(decryptModel);

                        //Lưu lại thằng session, mã hóa lại thông tin
                        var encryptModel = CryptoHelper.EncryptSessionCookie_User(JsonConvert.SerializeObject(model));

                        HttpContext.Session.SetString(SessionConfig.Kz_UserSession, encryptModel);
                    }
                    else
                    {
                        model = null;
                    }
                }
            }
            else
            {
                //Giải mã
                var decryptModel = CryptoHelper.DecryptSessionCookie_User(sessionValue);

                if (!string.IsNullOrWhiteSpace(decryptModel))
                {
                    model = JsonConvert.DeserializeObject<SessionModel>(decryptModel);
                }
                else
                {
                    model = null;
                }


            }

            return Task.FromResult(model);
        }
    }
}