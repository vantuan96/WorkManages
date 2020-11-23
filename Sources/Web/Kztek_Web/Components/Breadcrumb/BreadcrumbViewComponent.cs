using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Admin.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Components.Breadcrumb
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        private ISY_MenuFunctionService _SY_MenuFunctionService;
        private IHttpContextAccessor HttpContextAccessor;

        public BreadcrumbViewComponent(ISY_MenuFunctionService _SY_MenuFunctionService, IHttpContextAccessor HttpContextAccessor)
        {
            this._SY_MenuFunctionService = _SY_MenuFunctionService;
            this.HttpContextAccessor = HttpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(string controllername, string actionname)
        {
            //
            var user = await SessionCookieHelper.CurrentUser(HttpContextAccessor.HttpContext);

            //
            var model = new BreadcrumbModel();
            model.ControllerName = controllername;
            model.ActionName = actionname;

            var data = await _SY_MenuFunctionService.GetAllActiveByUserId(HttpContextAccessor.HttpContext, user);
            var dataList = data.ToList();

            model.CurrentView = dataList.FirstOrDefault(n => n.ControllerName.Equals(model.ControllerName) && n.ActionName.Equals(model.ActionName));

            var k = await LanguageHelper.GetMenuLanguageText(string.Format("{0}:{1}", controllername, actionname));

            if (!string.IsNullOrWhiteSpace(k))
            {
                if (model.CurrentView != null)
                {
                    model.CurrentView.MenuName = k;
                }
            }

            model.Breadcrumb = await _SY_MenuFunctionService.GetBreadcrumb(model.CurrentView != null ? model.CurrentView.Id : "", model.CurrentView != null ? model.CurrentView.ParentId : "", "");

            //List
            var listModel = new List<SelectListModel_Breadcrumb>();


            if (!string.IsNullOrWhiteSpace(model.Breadcrumb))
            {
                var id = model.Breadcrumb.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (id.Any())
                {
                    foreach (var item in id.Reverse())
                    {
                        var objF = dataList.FirstOrDefault(n => n.Id.Equals(item));
                        if (objF != null)
                        {
                            var childFunc = dataList.Where(n => n.ParentId == objF.Id && n.MenuType == "2");

                            if (!listModel.Any(n => n.ControllerName == objF.ControllerName && n.ActionName == objF.ActionName))
                            {
                                if (model.CurrentView != null)
                                {
                                    if (model.CurrentView.ControllerName == objF.ControllerName && model.CurrentView.ActionName == objF.ActionName)
                                    {

                                    }
                                    else
                                    {
                                        var menuname = await LanguageHelper.GetMenuLanguageText(string.Format("{0}:{1}", objF.ControllerName, objF.ActionName));

                                        listModel.Add(new SelectListModel_Breadcrumb
                                        {
                                            MenuName = objF.MenuName,
                                            ControllerName = objF.ControllerName,
                                            ActionName = objF.ActionName,
                                            isFolder = childFunc.Any() ? false : true
                                        });
                                    }
                                }
                                else
                                {
                                    var menuname = await LanguageHelper.GetMenuLanguageText(string.Format("{0}:{1}", objF.ControllerName, objF.ActionName));

                                    listModel.Add(new SelectListModel_Breadcrumb
                                    {
                                        MenuName = objF.MenuName,
                                        ControllerName = objF.ControllerName,
                                        ActionName = objF.ActionName,
                                        isFolder = childFunc.Any() ? false : true
                                    });
                                }
                            }
                        }
                    }
                }
            }

            model.Data = listModel;

            return View(model);
        }
    }
}