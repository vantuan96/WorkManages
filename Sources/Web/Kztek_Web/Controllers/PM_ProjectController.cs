using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.PM;
using Kztek_Service.Admin.Interfaces;
using Kztek_Service.Admin.Interfaces.PM;
using Kztek_Service.OneSignalr.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Controllers
{
    public class PM_ProjectController : Controller
    {
        private IPM_ProjectService _PM_ProjectService;
        private IPM_ComponentService _PM_ComponentService;
        private IPM_WorkService _PM_WorkService;
        private ISY_UserService _SY_UserService;
        private IOS_PlayerService _OS_PlayerService;
        private IReminderService _ReminderService;

        public PM_ProjectController(IPM_ProjectService _PM_ProjectService, IPM_ComponentService _PM_ComponentService, IPM_WorkService _PM_WorkService, ISY_UserService _SY_UserService, IOS_PlayerService _OS_PlayerService, IReminderService _ReminderService)
        {
            this._PM_ProjectService = _PM_ProjectService;
            this._PM_ComponentService = _PM_ComponentService;
            this._PM_WorkService = _PM_WorkService;
            this._SY_UserService = _SY_UserService;
            this._OS_PlayerService = _OS_PlayerService;
            this._ReminderService = _ReminderService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var data = await _PM_ProjectService.GetPaging("", 1, 20);

            ViewBag.keyValue = "";
            ViewBag.AuthValue = await AuthHelper.CheckAuthAction("PM_Project", this.HttpContext);

            return View(data);
        }

        public async Task<IActionResult> CreateNewProject(PM_Project model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    result = new MessageReport(false, "Vui lòng điền tên dự án");
                    return Json(result);
                }

                //
                var currentUser = await SessionCookieHelper.CurrentUser(this.HttpContext);

                //Gán
                model.Id = ObjectId.GenerateNewId().ToString();
                model.Description = "";
                model.Note = "";
                model.DateStart = DateTime.Now;
                model.DateEnd = DateTime.Now;
                model.DateCreated = DateTime.Now;
                model.Status = 0;
                model.UserCreatedId = currentUser != null ? currentUser.UserId : "";

                //
                result = await _PM_ProjectService.Create(model);
                if (result.isSuccess)
                {
                    result.Message = model.Id;
                }
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        public async Task<IActionResult> Update(string id)
        {
            var obj = await _PM_ProjectService.GetById(id);

            ViewBag.ProjectStatus = Select_ProjectStatus(obj != null ? obj.Status.ToString() : "0");

            return View(obj);
        }

        public async Task<IActionResult> ProjectSubmit(PM_ProjectAjax model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra tồn tại
                var existed = await _PM_ProjectService.GetById(model.Id);
                if (existed == null)
                {
                    result = new MessageReport(false, "Dự án không tồn tại");
                    return Json(result);
                }

                //Kiêm tra thiếu trường quy định

                //Gán lại
                existed.Title = model.Title;
                existed.DateEnd = Convert.ToDateTime(model.DateEnd);
                existed.DateStart = Convert.ToDateTime(model.DateStart);
                existed.Description = model.Description;
                existed.Note = model.Note;
                existed.Status = model.Status;

                result = await _PM_ProjectService.Update(existed);
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var result = await _PM_ProjectService.Delete(id);
            if (result.isSuccess)
            {
                //Lấy danh sách component để xóa schedule
                var list = await _PM_ComponentService.GetAllByProjectId(id);
                foreach (var item in list)
                {
                    RemoveSchedule(item);
                    _ReminderService.RemoveSchedule(item.Id, "project");
                }

                //Xóa component
                await _PM_ComponentService.DeleteByProjectId(id);

                //Xóa work
                await _PM_WorkService.DeleteByProjectId(id);
            }

            return Json(result);
        }

        public async Task<IActionResult> ComponentPartial(string projectid)
        {
            var data = await _PM_ComponentService.GetAllByProjectId(projectid);

            return PartialView(data);
        }

        public async Task<IActionResult> ComponentModal(AJAXModel_Modal obj)
        {
            var existed = await _PM_ComponentService.GetById(obj.idrecord);
            if (existed == null)
            {
                existed = new PM_Component()
                {
                    Code = "",
                    DateCreated = DateTime.Now,
                    DateEnd = DateTime.Now,
                    DateStart = DateTime.Now,
                    Description = "",
                    Id = ObjectId.GenerateNewId().ToString(),
                    Label = "",
                    Note = "",
                    ParentId = "0",
                    ProjectId = obj.idsub,
                    Status = 0,
                    Title = ""
                };
            }

            //Danh sách mẫu
            var model = new PM_ComponentModal()
            {
                Select_ProjectComponents = await Select_ProjectComponent(existed.ProjectId, existed.ParentId),
                Select_ProjectStatus = Select_ProjectStatus(existed.Status.ToString()),
                Data = existed,
                Data_Modal = obj
            };

            return PartialView(model);
        }

        public async Task<IActionResult> ComponentSubmit(PM_ComponentAjax model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra đã điền
                if (string.IsNullOrWhiteSpace(model.Code))
                {
                    result = new MessageReport(false, "Mã component không được để trống");
                    return Json(result);
                }

                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    result = new MessageReport(false, "Tên component không được để trống");
                    return Json(result);
                }

                //Kiểm tra có tồn tại
                var existed = await _PM_ComponentService.GetById(model.ComponentId);
                if (existed == null)
                {
                    //Kiểm tra
                    var existedCodeInProject = await _PM_ComponentService.GetByCode(model.ProjectId, model.Code);
                    if (existedCodeInProject != null)
                    {
                        result = new MessageReport(false, "Mã hiệu đã tồn tại trong dự án");
                        return Json(result);
                    }

                    //Thêm mới
                    existed = new PM_Component()
                    {
                        Code = model.Code,
                        DateCreated = DateTime.Now,
                        DateEnd = Convert.ToDateTime(model.DateEnd),
                        DateStart = Convert.ToDateTime(model.DateStart),
                        Description = model.Description,
                        Id = model.ComponentId,
                        Label = "",
                        Note = model.Note,
                        ParentId = model.ParentId,
                        ProjectId = model.ProjectId,
                        Status = Convert.ToInt32(model.Status),
                        Title = model.Title
                    };

                    result = await _PM_ComponentService.Create(existed);
                    if (result.isSuccess)
                    {
                        RegisterSchedule(existed);
                    }
                }
                else
                {
                    //Kiểm tra
                    var existedCodeInProject = await _PM_ComponentService.GetByCode(model.ProjectId, model.Code);
                    if (existedCodeInProject != null && existedCodeInProject.Id != model.ComponentId)
                    {
                        result = new MessageReport(false, "Mã hiệu đã tồn tại trong dự án");
                        return Json(result);
                    }

                    //Cập nhật
                    existed.Note = model.Note;
                    existed.ParentId = model.ParentId;
                    existed.Status = Convert.ToInt32(model.Status);
                    existed.Title = model.Title;
                    existed.Description = model.Description;
                    existed.DateEnd = Convert.ToDateTime(model.DateEnd);
                    existed.DateStart = Convert.ToDateTime(model.DateStart);

                    result = await _PM_ComponentService.Update(existed);
                    if (result.isSuccess)
                    {
                        ReRegisterSchedule(existed);
                    }
                }
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        public async Task<IActionResult> ComponentListPartial(string projectid)
        {
            var data = await _PM_ComponentService.GetAllByProjectId(projectid);

            return PartialView(data);
        }

        public async Task<IActionResult> ComponentParent(PM_ComponentAjax model)
        {
            var data = await _PM_ComponentService.GetById(model.ComponentId);
            return Json(data);
        }

        public async Task<IActionResult> ComponentDelete(PM_ComponentAjax model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                var objComponent = await _PM_ComponentService.GetById(model.ComponentId);

                //Xóa component
                result = await _PM_ComponentService.Delete(model.ComponentId);
                if (result.isSuccess)
                {
                    //Xóa work ăn theo
                    await _PM_WorkService.DeleteByComponentId(model.ComponentId);

                    //
                    RemoveSchedule(objComponent);

                    //
                    _ReminderService.RemoveSchedule(model.ComponentId, "project");
                }
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        public async Task<IActionResult> ComponentMComplete(PM_ComponentAjax model)
        {
            //
            var result = await _PM_ComponentService.MComplete(model.ComponentId);
            if (result.isSuccess == true)
            {
                await _PM_WorkService.MCompleteByComponentId(model.ComponentId);
            }

            return Json(result);
        }

        public async Task<IActionResult> WorkModal(AJAXModel_Modal model)
        {
            var data = await _PM_WorkService.GetAllByComponentId(model.idrecord);

            var k = data.Select(n => n.UserId);

            var select = string.Join(',', k);

            var dt = new PM_WorkModal()
            {
                Data = data,
                Select_User = await Select_User(select),
                Data_Modal = model
            };

            return PartialView(dt);
        }

        public async Task<IActionResult> WorkSubmit(PM_WorkAjax model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Kiểm tra có chọn người tham gia
                if (model.UserIds == "null" || string.IsNullOrWhiteSpace(model.UserIds))
                {
                    result = new MessageReport(false, "Vui lòng người tham gia");
                    return Json(result);
                }

                //Xóa mapping
                await _PM_WorkService.DeleteByComponentId(model.ComponentId);

                //Lấy component
                var objComponent = await _PM_ComponentService.GetById(model.ComponentId);

                //Split userid
                var ids = model.UserIds.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var item in ids)
                {
                    var k = new PM_Work()
                    {
                        ComponentId = model.ComponentId,
                        DateCompleted = DateTime.Now,
                        DateCreated = DateTime.Now,
                        DateRealStart = objComponent != null ? objComponent.DateStart : DateTime.Now,
                        Id = ObjectId.GenerateNewId().ToString(),
                        IsCompleted = false,
                        IsOnScheduled = false,
                        ProjectId = model.ProjectId,
                        UserId = item
                    };

                    result = await _PM_WorkService.Create(k);
                }

            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }



        public async Task<IActionResult> HomePartial()
        {
            //Lấy người dùng hiện tại
            var user = await SessionCookieHelper.CurrentUser(this.HttpContext);

            //Lấy danh sách công việc của người đó trong các dự án chưa làm xong
            var dataWork = await _PM_WorkService.GetAllUnfinishedWorkByUserId(user.UserId);

            //Lấy danh sách dự án theo công việc đó
            var dataProject = await _PM_ProjectService.GetProjectsByIds(dataWork.Select(n => n.ProjectId).ToList());

            //Mapping
            var model = new PM_ProjectHomeModel()
            {
                Data_Project = dataProject
            };

            return PartialView(model);
        }

        public async Task<IActionResult> HomeComponentPartial(string projectid)
        {
            //Lấy người dùng hiện tại
            var user = await SessionCookieHelper.CurrentUser(this.HttpContext);

            //Lấy dự án
            var objProject = await _PM_ProjectService.GetById(projectid);

            //Lấy danh sách công việc của người đó trong các dự án chưa làm xong
            var dataWork = await _PM_WorkService.GetAllUnfinishedWorkByUserId_ProjectId(user.UserId, projectid);

            //Lấy danh sách component theo wỏk
            var dataComponent = await _PM_ComponentService.GetAllCurrentByIds(dataWork.Select(n => n.ComponentId).ToList());

            ViewBag.UserIdValue = user.UserId;
            ViewBag.projectidValue = projectid;
            ViewBag.objProjectValue = objProject;

            return PartialView(dataComponent);
        }

        public async Task<IActionResult> ComponentComplete(PM_WorkComplete model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Lấy bản ghi phân công
                var obj = await _PM_WorkService.GetByProjectId_ComponentId_UserId(model.ProjectId, model.ComponentId, model.UserId);

                //
                if (obj == null)
                {
                    result = new MessageReport(false, "Không tồn tại component với bạn");
                    return Json(result);
                }

                //Cập nhật
                obj.IsCompleted = true;
                obj.DateCompleted = DateTime.Now;
                obj.IsOnScheduled = true;

                //Kiểm tra có đúng hạn
                var objComponent = await _PM_ComponentService.GetById(model.ComponentId);
                if (objComponent == null)
                {
                    result = new MessageReport(false, "Component không tồn tại");
                    return Json(result);
                }

                //Kiểm tra
                if (obj.DateCompleted > objComponent.DateEnd)
                {
                    obj.IsOnScheduled = false;
                }

                result = await _PM_WorkService.Update(obj);
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        public async Task<IActionResult> OneSignalrRegister(OneSignalrSubmit model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Lấy người dùng hiện tại
                var user = await SessionCookieHelper.CurrentUser(this.HttpContext);
                if (user == null)
                {
                    result = new MessageReport(false, "Chưa đăng nhập");
                    return Json(result);
                }

                if (string.IsNullOrWhiteSpace(model.PlayerId))
                {
                    result = new MessageReport(false, "Chưa có Player id");
                    return Json(result);
                }

                //
                var existed = await _OS_PlayerService.GetByPlayerId_UserId(model.PlayerId, user.UserId);
                if (existed == null)
                {
                    existed = new Kztek_Model.Models.OneSignalr.OS_Player()
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        UserId = user.UserId,
                        PlayerId = model.PlayerId
                    };

                    result = await _OS_PlayerService.Create(existed);
                }
                else
                {
                    result = new MessageReport(true, "Đã đăng ký");
                }
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        public async Task<IActionResult> SendMessageModal(AJAXModel_Modal model)
        {
            //
            var objProject = await _PM_ProjectService.GetById(model.idrecord);

            //Lấy danh sách user tham gia dự án
            var objWork = await _PM_WorkService.GetAllByProjectId(model.idrecord);
            var userids = objWork.Select(n => n.UserId).Distinct().ToList();

            var obj = new OneSignalrMessage()
            {
                Description = "",
                Title = "Nhắc nhở dự án: " + objProject.Title,
                Id = model.idrecord,
                PlayerIds = new string[] { },
                View = "ProjectPage"
            };

            var cus = new OS_MessageModal()
            {
                Data = obj,
                Data_Modal = model,
                Select_Users = await Select_User(string.Join(',', userids))
            };

            return PartialView(cus);
        }

        public async Task<IActionResult> MessageSubmit(OneSignalrMessage model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Lấy danh sách cần gửi
                if (string.IsNullOrWhiteSpace(model.UserIds))
                {
                    result = new MessageReport(false, "Chưa có người để gửi");
                    return Json(result);
                }

                //
                var k = model.UserIds.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                var players = await _OS_PlayerService.GetPlayerIdsByUserIds(k);

                model.PlayerIds = players.Where(n => n.PlayerId != null).Select(n => n.PlayerId).ToArray();
                model.View = "ProjectPage";

                //
                result = await _OS_PlayerService.SendMessage(model);
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
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

        private async Task<SelectListModel_Chosen> Select_ProjectComponent(string projectid, string select)
        {
            var model = new SelectListModel_Chosen()
            {
                Data = await GetMenuList(projectid),
                IdSelectList = "ParentId",
                isMultiSelect = false,
                Placeholder = "",
                Selecteds = !string.IsNullOrWhiteSpace(select) ? select : ""
            };

            return model;
        }

        private async Task<List<SelectListModel>> GetMenuList(string projectid)
        {
            var list = new List<SelectListModel>
            {
                new SelectListModel { ItemValue  = "0", ItemText = "- Chọn component -" }
            };

            var MenuList = await _PM_ComponentService.GetAllCustomByProjectId(projectid);
            var parent = MenuList.Where(c => c.ParentId == "0").ToList();
            if (parent.Any())
            {
                foreach (var item in parent)
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    list.Add(new SelectListModel { ItemValue = item.Id, ItemText = item.Title });

                    var listChild = MenuList.Where(c => c.ParentId == item.Id).ToList();

                    //Gọi action để lấy danh sách submenu theo id
                    if (listChild.Any())
                    {
                        Children(listChild, MenuList, list, item);
                    }

                    list.Add(new SelectListModel { ItemValue = "", ItemText = "-----" });

                }
            }
            return list;
        }

        private void Children(List<PM_Component_Submit> listChild, List<PM_Component_Submit> allFunction, List<SelectListModel> lst, PM_Component_Submit itemParent)
        {
            //Kiểm tra có dữ liệu chưa
            if (listChild.Any())
            {
                foreach (var item in listChild)
                {
                    //Nếu có thì duyệt tiếp để lưu vào list
                    lst.Add(new SelectListModel { ItemValue = item.Id, ItemText = itemParent.Title + " \\ " + item.Title });

                    //Gọi action để lấy danh sách submenu theo id
                    var child = allFunction.Where(c => c.ParentId == item.Id).ToList();

                    //Gọi action để lấy danh sách submenu theo id
                    if (child.Any())
                    {
                        item.Title = itemParent.Title + " \\ " + item.Title;
                        Children(child, allFunction, lst, item);
                    }
                }
            }
        }

        private async Task<SelectListModel_Chosen> Select_User(string select)
        {
            var model = new SelectListModel_Chosen()
            {
                Data = await Data_User(),
                IdSelectList = "slUser",
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

        private async Task ReRegisterSchedule(PM_Component model)
        {
            await RemoveSchedule(model);
            await RegisterSchedule(model);
        }

        private async Task RegisterSchedule(PM_Component model)
        {
            await _ReminderService.RegisterSchedule(model.Id, "project", model.DateEnd.AddMinutes(-15).ToString("yyyy/MM/dd HH:mm:ss"), string.Format("Nhắc nhở check hoàn thành component: {0}", model.Code));
        }

        private async Task RemoveSchedule(PM_Component model)
        {
            await _ReminderService.RemoveSchedule(model.Id, "project");
        }
    }
}
