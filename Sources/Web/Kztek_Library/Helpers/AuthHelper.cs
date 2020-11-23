using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek.Security;
using Kztek_Core.Models;
using Kztek_Library.Configs;
using Kztek_Library.Functions;
using Kztek_Library.Models;
using Kztek_Model.Models;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;

namespace Kztek_Library.Helpers
{
    public class AuthHelper
    {
        public static Task<AuthActionModel> CheckAuthAction(string controller, HttpContext context)
        {
            var Auth = new AuthActionModel();

            //Get list auth of current user
            var currentUser = SessionCookieHelper.CurrentUser(context).Result;
            if (currentUser != null)
            {
                //Get list all menufunction by controller
                var menus = MenuFunctionByUserId(currentUser, context).Result;

                if (currentUser.isAdmin)
                {
                    Auth.Create_Auth = 1;
                    Auth.Update_Auth = 1;
                    Auth.Delete_Auth = 1;
                }
                else
                {
                    //Action Create
                    var objCreate = menus.FirstOrDefault(n => n.ControllerName == controller && n.ActionName == "Create");
                    if (objCreate != null)
                    {
                        Auth.Create_Auth = 1;
                    }

                    //Action Update
                    var objUpdate = menus.FirstOrDefault(n => n.ControllerName == controller && n.ActionName == "Update");
                    if (objUpdate != null)
                    {
                        Auth.Update_Auth = 1;
                    }

                    //Action Delete
                    var objDelete = menus.FirstOrDefault(n => n.ControllerName == controller && n.ActionName == "Delete");
                    if (objDelete != null)
                    {
                        Auth.Delete_Auth = 1;
                    }
                }


            }

            return Task.FromResult(Auth);
        }

        public static Task<List<SY_MenuFunction>> MenuFunctionByUserId(SessionModel user, HttpContext context)
        {
            var connecttype = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultType").Result;

            //
            var identify = string.Format(CacheConfig.Kz_User_MenuFunctionCache_Key, user.UserId, SecurityModel.Cache_Key);

            var modelCache = new List<SY_MenuFunction>();

            //var cache = context.RequestServices.GetService<IMemoryCache>();

            var existed = CacheFunction.TryGet<List<SY_MenuFunction>>(context, identify, out modelCache);

            //var k = cache.Get<List<SY_MenuFunction>>(identify);

            if (existed == false)
            {
                if (user.isAdmin)
                {
                    //Làm việc với mông => không thể dùng cmd giống như sqlserve, mysql
                    switch (connecttype)
                    {
                        case DatabaseModel.MONGO:

                            var filter = Builders<SY_MenuFunction>.Filter.Eq(n => n.Active, true);

                            var kd = MongoHelper.GetConnect<SY_MenuFunction>().FindAsync(filter);

                            modelCache = kd.Result.ToList();

                            break;

                        default:

                            var cmdMenus = "SELECT * FROM sy_menufunction WHERE Active = 1";

                            modelCache = Kztek_Library.Helpers.DatabaseHelper.ExcuteCommandToList<SY_MenuFunction>(cmdMenus);

                            break;
                    }
                }
                else
                {
                    switch (connecttype)
                    {
                        case DatabaseModel.MONGO:

                            var filterRole = Builders<SY_Map_User_Role>.Filter.Eq(n => n.UserId, user.UserId);
                            var moRoles = MongoHelper.GetConnect<SY_Map_User_Role>().FindAsync(filterRole).Result.ToList();

                            var filterMenu = Builders<SY_Map_Role_Menu>.Filter.In(n => n.RoleId, moRoles.Select(n => n.RoleId));
                            var moMenus = MongoHelper.GetConnect<SY_Map_Role_Menu>().FindAsync(filterMenu).Result.ToList();

                            var filterQueryMenu = Builders<SY_MenuFunction>.Filter.In(n => n.Id, moMenus.Select(n => n.MenuId));
                            modelCache = MongoHelper.GetConnect<SY_MenuFunction>().FindAsync(filterQueryMenu).Result.ToList();

                            break;

                        default:

                            var cmdRole = string.Format("SELECT * FROM sy_map_user_role WHERE UserId = '{0}'", user.UserId);

                            var roles = Kztek_Library.Helpers.DatabaseHelper.ExcuteCommandToList<SY_Map_User_Role>(cmdRole);

                            var str_roles = new List<string>();
                            foreach (var item in roles)
                            {
                                str_roles.Add(string.Format("'{0}'", item.RoleId));
                            }

                            //Danh sách menu của tài khoản với roleids = > danh sách menu
                            var cmdRoleMenus = string.Format("SELECT * FROM sy_map_role_menu WHERE RoleId IN ({0})", roles.Any() ? string.Join(",", str_roles) : "'0'");

                            var rolemenus = Kztek_Library.Helpers.DatabaseHelper.ExcuteCommandToList<SY_Map_Role_Menu>(cmdRoleMenus);

                            //Lấy danh sách menu quyền
                            var menuids = "";
                            var count1 = 0;
                            foreach (var item in rolemenus)
                            {
                                count1++;
                                menuids += string.Format("'{0}'{1}", item.MenuId, count1 == rolemenus.Count ? "" : ",");
                            }

                            var cmdMenus = string.Format("SELECT * FROM sy_menufunction WHERE Active = 1 AND Id IN ({0})", string.IsNullOrWhiteSpace(menuids) ? "'0'" : menuids);

                            modelCache = Kztek_Library.Helpers.DatabaseHelper.ExcuteCommandToList<SY_MenuFunction>(cmdMenus);

                            break;
                    }
                }

                //Save lại vào cache
                if (modelCache == null)
                {
                    modelCache = new List<SY_MenuFunction>();
                }

                CacheFunction.Add<List<SY_MenuFunction>>(context, identify, modelCache, CacheConfig.Kz_User_MenuFunctionCache_Time);
                //cache.Set<List<SY_MenuFunction>>(identify, modelCache, DateTime.Now.AddHours(8));
            }

            return Task.FromResult(modelCache);
        }
    }

}