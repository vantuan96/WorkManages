using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;
using Kztek_Service.Admin.Interfaces;
using Kztek_Service.Admin.Interfaces.WM;
using Kztek_Service.OneSignalr.Interfaces;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Kztek_Web.Controllers
{
    public class WM_TaskController : Controller
    {
        private IWM_TaskService _WM_TaskService;
        private ISY_UserService _SY_UserService;
        private IOS_PlayerService _OS_PlayerService;
        private IReminderService _ReminderService;
        private ISY_ReminderSystemService _SY_ReminderSystemService;

        public WM_TaskController(IWM_TaskService _WM_TaskService, ISY_UserService _SY_UserService, IOS_PlayerService _OS_PlayerService, IReminderService _ReminderService, ISY_ReminderSystemService _SY_ReminderSystemService)
        {
            this._WM_TaskService = _WM_TaskService;
            this._SY_UserService = _SY_UserService;
            this._OS_PlayerService = _OS_PlayerService;
            this._ReminderService = _ReminderService;
            this._SY_ReminderSystemService = _SY_ReminderSystemService;
        }

        [CheckSessionCookie]
        public async Task<IActionResult> Index(int page = 1, string export = "0")
        {
            var gridmodel = await _WM_TaskService.GetPaging("", page, 10);

            ViewBag.keyValue = "";
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("WM_Task", this.HttpContext);

            return View(gridmodel);
        }


        [CheckSessionCookie]
        [HttpGet]
        public async Task<IActionResult> Create(WM_TaskSubmit model)
        {
            model = model == null ? new WM_TaskSubmit() : model;

            ViewBag.Select_User = await Select_User("");
            ViewBag.Select_Status = Select_ProjectStatus("0");

            return View(await Task.FromResult(model));
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Create(WM_TaskSubmit model, bool SaveAndCountinue = false)
        {
            ViewBag.Select_User = await Select_User("");
            ViewBag.Select_Status = Select_ProjectStatus(model.Status.ToString());
            var users = new string[] { };

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var currentUser = await SessionCookieHelper.CurrentUser(this.HttpContext);

            var obj = new WM_Task()
            {
                DateCreated = DateTime.Now,
                DateEnd = Convert.ToDateTime(model.DateEnd),
                DateStart = Convert.ToDateTime(model.DateStart),
                Description = model.Description,
                Id = ObjectId.GenerateNewId().ToString(),
                Title = model.Title,
                Status = 0,
                Note = "",
                UserCreatedId = currentUser != null ? currentUser.UserId : ""
            };

            if (model.UserIds.Any())
            {
                //Lấy danh sách mới
                //users = model.UserIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in model.UserIds)
                {
                    var userTask = new WM_TaskUser()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        TaskId = obj.Id,
                        UserId = item,
                        IsCompleted = false,
                        IsOnScheduled = false,
                        DateCompleted = DateTime.Now,
                        DateCreated = DateTime.Now
                    };

                    await _WM_TaskService.CreateUserTask(userTask);
                }

                //Thêm người tạo dự án
                users.Append(obj.UserCreatedId);
            }

            //Thực hiện thêm mới
            var result = await _WM_TaskService.Create(obj);
            if (result.isSuccess)
            {
                SendMessage(obj, users.ToList());

                RegisterSchedule(obj);

                //Lưu lại log trên server
                _SY_ReminderSystemService.Create(new Kztek_Model.Models.SY_ReminderSystem()
                {
                    Id = obj.Id,
                    Type = "task",
                    isDone = false,
                    DateReminder = obj.DateEnd.AddMinutes(-15)
                });

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
            var model = await _WM_TaskService.GetCustomById(id);

            ViewBag.Select_User = await Select_User(string.Join(',', model.UserIds));
            ViewBag.Select_Status = Select_ProjectStatus(model.Status.ToString());

            return View(model);
        }

        [CheckSessionCookie]
        [HttpPost]
        public async Task<IActionResult> Update(WM_TaskSubmit model)
        {
            ViewBag.Select_User = await Select_User(string.Join(',', model.UserIds));
            ViewBag.Select_Status = Select_ProjectStatus(model.Status.ToString());

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var oldObj = await _WM_TaskService.GetById(model.Id);
            if (oldObj == null)
            {
                ModelState.AddModelError("", "Bản ghi không tồn tại");
                return View(model);
            }

            oldObj.DateEnd = Convert.ToDateTime(model.DateEnd);
            oldObj.DateStart = Convert.ToDateTime(model.DateStart);
            oldObj.Description = model.Description;
            oldObj.Title = model.Title;
            oldObj.Status = model.Status;

            // if (!string.IsNullOrWhiteSpace(model.UserIds))
            // {
            //     //Xóa người tham gia đã có từ trước
            //     await _WM_TaskService.DeleteUserTasksByTaskId(oldObj.Id);

            //     //Lấy danh sách mới
            //     var users = model.UserIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
            //     foreach (var item in users)
            //     {
            //         var userTask = new WM_TaskUser()
            //         {
            //             Id = ObjectId.GenerateNewId().ToString(),
            //             TaskId = model.Id,
            //             UserId = item,
            //             IsCompleted = false,
            //             IsOnScheduled = false,
            //             DateCompleted = DateTime.Now,
            //             DateCreated = DateTime.Now
            //         };

            //         await _WM_TaskService.CreateUserTask(userTask);
            //     }
            // }

            var result = await _WM_TaskService.Update(oldObj);
            if (result.isSuccess)
            {
                if (oldObj.Status == 1)
                {
                    RemoveSchedule(oldObj);
                } 
                else
                {
                    ReRegisterSchedule(oldObj);
                }

                var objLogRe = await _SY_ReminderSystemService.GetById(oldObj.Id);
                if (objLogRe == null)
                {
                    _SY_ReminderSystemService.Create(new Kztek_Model.Models.SY_ReminderSystem()
                    {
                        Id = oldObj.Id,
                        Type = "task",
                        isDone = oldObj.Status == 1 ? true : false,
                        DateReminder = oldObj.DateEnd.AddMinutes(-15)
                    });
                } 
                else
                {
                    //Lưu lại log trên server
                    _SY_ReminderSystemService.Update(new Kztek_Model.Models.SY_ReminderSystem()
                    {
                        Id = oldObj.Id,
                        Type = "task",
                        isDone = oldObj.Status == 1 ? true : false,
                        DateReminder = oldObj.DateEnd.AddMinutes(-15)
                    });
                }

               

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
            var objTask = await _WM_TaskService.GetById(id);

            var result = await _WM_TaskService.Delete(id);
            if (result.isSuccess)
            {
                RemoveSchedule(objTask);

                _SY_ReminderSystemService.Delete(id);
            }

            return Json(result);
        }

        private async Task<SelectListModel_Chosen> Select_User(string select)
        {
            var model = new SelectListModel_Chosen()
            {
                Data = await Data_User(),
                IdSelectList = "UserIds",
                isMultiSelect = true,
                Placeholder = "",
                Selecteds = !string.IsNullOrWhiteSpace(select) ? select : ""
            };

            return model;
        }

        private async Task<List<SelectListModel>> Data_User()
        {
            var cu = new List<SelectListModel>();

            var data = await _SY_UserService.GetAllActive();
            foreach (var item in data)
            {
                cu.Add(new SelectListModel()
                {
                    ItemValue = item.Id,
                    ItemText = item.Username + "(" + item.Name + ")"
                });
            }

            return cu;
        }

        private SelectListModel_Chosen Select_ProjectStatus(string select)
        {
            var model = new SelectListModel_Chosen()
            {
                Data = StaticList.ProjectStatus(),
                IdSelectList = "Status",
                isMultiSelect = false,
                Placeholder = "",
                Selecteds = !string.IsNullOrWhiteSpace(select) ? select : ""
            };

            return model;
        }

        public async Task<IActionResult> HomeTaskPartial()
        {
            //Lấy người dùng hiện tại
            var user = await SessionCookieHelper.CurrentUser(this.HttpContext);

            //
            var data = await _WM_TaskService.GetTaskByUserId(user != null ? user.UserId : "");

            return PartialView(data);
        }

        public async Task<IActionResult> HomeCompleteTask(WM_TaskComplete model)
        {

            var result = new MessageReport(false, "Có lỗi xảy ra");
            var user = await SessionCookieHelper.CurrentUser(this.HttpContext);
            model.UserId = user != null ? user.UserId : "";

            try
            {
                //Lấy task
                var objTask = await _WM_TaskService.GetById(model.TaskId);
                if (objTask == null)
                {
                    result = new MessageReport(false, "Công việc của bạn không tồn tại");
                    return Json(result);
                }

                //Task user
                var userTask = await _WM_TaskService.GetByTaskId_UserId(model.TaskId, model.UserId);
                if (userTask == null)
                {
                    result = new MessageReport(false, "Công việc của bạn không tồn tại");
                    return Json(result);
                }

                //Check công việc hoàn thành
                userTask.IsCompleted = true;
                userTask.DateCompleted = DateTime.Now;
                userTask.IsOnScheduled = true;

                if (userTask.DateCompleted > objTask.DateEnd)
                {
                    userTask.IsOnScheduled = false;
                }

                result = await _WM_TaskService.UpdateUserTask(userTask);
                if (result.isSuccess)
                {
                    var userTasks = await _WM_TaskService.GetUserTasksByTaskId(objTask.Id);
                    var userIds = userTasks.Select(n => n.UserId).ToList();
                    userIds.Add(objTask.UserCreatedId);

                    SendMessageComplete(objTask, userIds, model.UserId);

                    RemoveSchedule(objTask);
                }
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }


        private async Task<MessageReport> SendMessage(WM_Task task, List<string> users)
        {
            var result = new MessageReport(false, "error");

            try
            {

                //Lấy Players 
                var players = await _OS_PlayerService.GetPlayerIdsByUserIds(users);

                //Gửi
                var model = new OneSignalrMessage()
                {
                    Id = "",
                    Title = string.Format("Công việc: {0}", task.Title),
                    Description = string.Format("Công việc mới được giao cho bạn"),
                    UserIds = "",
                    PlayerIds = players.Select(n => n.PlayerId).ToArray(),
                    View = "TaskPage"
                };

                result = await _OS_PlayerService.SendMessage(model);
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        private async Task<MessageReport> SendMessageComplete(WM_Task task, List<string> users, string userCompleteId)
        {
            var result = new MessageReport(false, "error");

            try
            {

                //Lấy người hoàn thiện
                var objUser = await _SY_UserService.GetById(userCompleteId);

                //Lấy Players 
                var players = await _OS_PlayerService.GetPlayerIdsByUserIds(users);

                //Gửi
                var model = new OneSignalrMessage()
                {
                    Id = "",
                    Title = string.Format("Công việc: {0}", task.Title),
                    Description = string.Format("Công việc được check hoàn thành bởi {0}", objUser != null ? objUser.Username : ""),
                    UserIds = "",
                    PlayerIds = players.Select(n => n.PlayerId).ToArray(),
                    View = "TaskPage"
                };

                result = await _OS_PlayerService.SendMessage(model);
            }
            catch (System.Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }
        private async Task ReRegisterSchedule(WM_Task model)
        {
            await RemoveSchedule(model);
            await RegisterSchedule(model);
        }

        private async Task RegisterSchedule(WM_Task model)
        {
            await _ReminderService.RegisterSchedule(model.Id, "task", model.DateEnd.AddMinutes(-15).ToString("yyyy/MM/dd HH:mm:ss"), string.Format("Nhắc nhở check hoàn thành công việc: {0}", model.Title));
        }

        private async Task RemoveSchedule(WM_Task model)
        {
            await _ReminderService.RemoveSchedule(model.Id, "task");
        }
    }
}