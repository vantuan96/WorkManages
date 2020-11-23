using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Extensions;
using Kztek_Library.Helpers;
using Kztek_Model.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MySql.Data.MySqlClient;

namespace Kztek_Web.Attributes
{
    public class CheckSessionCookie : Attribute, IAuthorizationFilter
    {
        IHttpContextAccessor HttpContextAccessor;
        public CheckSessionCookie()
        {
            HttpContextAccessor = new HttpContextAccessor();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var connecttype = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultType").Result;

            //Check có session / cookie
            var currentUser = SessionCookieHelper.CurrentUser(HttpContextAccessor.HttpContext).Result;
            if (currentUser == null)
            {
                context.Result = new RedirectResult("/Login/Index");
                return;
            }

            switch (connecttype)
            {
                case DatabaseModel.MONGO:

                    var filter = Builders<SY_User>.Filter.Eq(n => n.Id, currentUser.UserId);

                    var kd = MongoHelper.GetConnect<SY_User>().FindAsync(filter).Result.FirstOrDefault();

                    if (kd == null)
                    {
                        context.Result = new RedirectResult("/Login/Index");
                        return;
                    }

                    break;

                default:

                    //Check tk tồn tại trong db
                    var cmdq = string.Format("SELECT * FROM sy_user WHERE Id = '{0}'", currentUser.UserId);

                    var result = Kztek_Library.Helpers.DatabaseHelper.ExcuteCommandToBool(cmdq);

                    if (result == false)
                    {
                        context.Result = new RedirectResult("/Login/Index");
                        return;
                    }

                    break;
            }

            //Check quyền
            if (currentUser.isAdmin == false)
            {
                //Id menu hiện tại
                var controller = (string)context.RouteData.Values["Controller"];
                var action = (string)context.RouteData.Values["Action"];

                var modelCache = AuthHelper.MenuFunctionByUserId(currentUser, HttpContextAccessor.HttpContext).Result;

                if (!modelCache.Any(n => n.ControllerName == controller && n.ActionName == action))
                {
                    context.Result = new RedirectResult("/Home/NotAuthorize");
                    return;
                }
            }
        }
    }
}