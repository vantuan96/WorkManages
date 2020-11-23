using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;
using Kztek_Service.Admin.Interfaces.WM;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Kztek_Web.Controllers
{
    public class WM_DiaryController : Controller
    {
        private IWM_DiaryService _WM_DiaryService;

        public WM_DiaryController(IWM_DiaryService _WM_DiaryService)
        {
            this._WM_DiaryService = _WM_DiaryService;
        }

        public async Task<IActionResult> HomeDiaryPartial(string scheduleid)
        {
            //Lấy người dùng hiện tại
            var currentUser = await SessionCookieHelper.CurrentUser(this.HttpContext);

            var list = await _WM_DiaryService.GetAllByUserId_ScheduleId(currentUser != null ? currentUser.UserId : "", scheduleid);

            return PartialView(list);
        }

        public async Task<IActionResult> HomeDiaryModal(AJAXModel_Modal obj)
        {
            var existed = await _WM_DiaryService.GetById(obj.idrecord);
            if (existed == null)
            {
                existed = new WM_Diary()
                {
                    DateCreated = DateTime.Now,
                    Description = "",
                    Id = ObjectId.GenerateNewId().ToString(),
                    ScheduleId = obj.idsub,
                    Title = "",
                };
            }

            var model = new WM_DiaryModel()
            {
                Data = existed,
                Data_Modal = obj
            };

            return PartialView(model);
        }

        public async Task<IActionResult> HomeDiarySubmit(WM_Diary model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                //Lấy người dùng hiện tại
                var currentUser = await SessionCookieHelper.CurrentUser(this.HttpContext);

                //Kiểm tra đã điền
                if (string.IsNullOrWhiteSpace(model.Title))
                {
                    result = new MessageReport(false, "Tên component không được để trống");
                    return Json(result);
                }

                //Kiểm tra có tồn tại
                var existed = await _WM_DiaryService.GetById(model.Id);
                if (existed == null)
                {
                    //Thêm mới
                    existed = new WM_Diary()
                    {
                        DateCreated = DateTime.Now,
                        Description = model.Description,
                        Id = model.Id,
                        Title = model.Title,
                        ScheduleId = model.ScheduleId,
                        UserId = currentUser != null ? currentUser.UserId : ""
                    };

                    result = await _WM_DiaryService.Create(existed);
                }
                else
                {
                    //Cập nhật
                    existed.Title = model.Title;
                    existed.Description = model.Description;

                    result = await _WM_DiaryService.Update(existed);
                }
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return Json(result);
        }

        public async Task<IActionResult> HomeDiaryDelete(string id)
        {
            var result = await _WM_DiaryService.Delete(id);
            return Json(result);
        }
    }
}