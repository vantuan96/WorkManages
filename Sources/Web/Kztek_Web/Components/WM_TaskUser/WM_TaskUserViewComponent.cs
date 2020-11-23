using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Admin;
using Kztek_Service.Admin.Interfaces;
using Kztek_Service.Admin.Interfaces.PM;
using Kztek_Service.Admin.Interfaces.WM;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Components.ChosenSelect
{
    public class WM_TaskUserViewComponent : ViewComponent
    {
        private IHttpContextAccessor HttpContextAccessor;
        private IWM_TaskService _WM_TaskService;
        private ISY_UserService _SY_UserService;

        public WM_TaskUserViewComponent(IWM_TaskService _WM_TaskService, ISY_UserService _SY_UserService)
        {
            this._WM_TaskService = _WM_TaskService;
            this._SY_UserService = _SY_UserService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string taskid)
        {
            var userTasks = await _WM_TaskService.GetUserTasksByTaskId(taskid);

            //Lấy người dùng
            var users = await _SY_UserService.GetAllByIds(userTasks.Select(n => n.UserId).ToList());

            //Mapping list
            var custom = new List<WM_TaskUserCustomView>();

            foreach (var item in users)
            {
                var mo = new WM_TaskUserCustomView() {
                    Username = item.Username,
                    IsCompleted = false,
                    IsOnScheduled = false,
                };

                var work = userTasks.FirstOrDefault(n => n.UserId == item.Id);
                if (work != null)
                {
                    mo.IsCompleted = work.IsCompleted;
                    mo.IsOnScheduled = work.IsOnScheduled;
                }

                custom.Add(mo);
            }

            return View(custom);
        }
    }
}