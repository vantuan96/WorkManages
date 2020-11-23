using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Model.Models.WM;
using Kztek_Service.Api.Interfaces;
using MongoDB.Bson;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class ScheduleService : IScheduleService
    {
        private IWM_ScheduleRepository _WM_ScheduleRepository;
        private IWM_DiaryRepository _WM_DiaryRepository;

        public ScheduleService(IWM_ScheduleRepository _WM_ScheduleRepository, IWM_DiaryRepository _WM_DiaryRepository)
        {
            this._WM_ScheduleRepository = _WM_ScheduleRepository;
            this._WM_DiaryRepository = _WM_DiaryRepository;
        }

        public async Task<MessageReport> AddDiary(WM_DiaryMobile model)
        {
            var obj = new WM_Diary()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Title = model.Title,
                Description = model.Description,
                DateCreated = DateTime.Now,
                ScheduleId = model.ScheduleId,
                UserId = model.UserId
            };

            return await _WM_DiaryRepository.Add(obj);
        }

        public async Task<MessageReport> DeleteDiary(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await _WM_DiaryRepository.GetOneById(id);
            if (obj != null)
            {
                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + id + "' }");
                query.AppendLine("}");

                return await _WM_DiaryRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> EditDiary(WM_DiaryMobile model)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            try
            {
                var existed = await _WM_DiaryRepository.GetOneById(model.Id);
                if (existed == null)
                {
                    result = new MessageReport(false, "Bản ghi không tồn tại");
                    return result;
                }

                existed.Title = model.Title;
                existed.Description = model.Description;

                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + existed.Id + "' }");
                query.AppendLine("}");

                return await _WM_DiaryRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), existed);
            }
            catch (Exception ex)
            {
                result = new MessageReport(false, ex.Message);
            }

            return result;
        }

        public async Task<WM_ScheduleMobile> GetCurrentScheduleByUserId(string userid)
        {
            var model = new WM_ScheduleMobile();

            try
            {
                //Lấy được kế hoạch của tuần này
                var objSchedule = await GetCurrentSchedule();

                if (objSchedule == null)
                {
                    model = new WM_ScheduleMobile()
                    {
                        Id = "",
                        Title = "Chưa có kế hoạch công việc tuần này",
                        DateStart = "",
                        DateEnd = "",
                        Description = "",
                        Diaries = new System.Collections.Generic.List<WM_DiaryMobile>()
                    };

                    return model;
                }

                //Mapping lại model
                model = new WM_ScheduleMobile()
                {
                    Id = objSchedule.Id,
                    Title = objSchedule.Title,
                    Description = objSchedule.Description,
                    DateStart = objSchedule.DateStart.ToString("dd/MM/yyyy"),
                    DateEnd = objSchedule.DateEnd.ToString("dd/MM/yyyy"),
                    Diaries = new List<WM_DiaryMobile>()
                };

                //Lấy danh sách diary của user
                var diaries = await GetDiariesByScheduleId_UserId(objSchedule.Id, userid);

                foreach (var item in diaries)
                {
                    model.Diaries.Add(new WM_DiaryMobile()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Description = string.IsNullOrWhiteSpace(item.Description) ? "" : item.Description
                    });
                }
            }
            catch (Exception ex)
            {
                model = new WM_ScheduleMobile()
                {
                    Id = "",
                    Title = ex.Message,
                    DateStart = "",
                    DateEnd = "",
                    Description = "",
                    Diaries = new System.Collections.Generic.List<WM_DiaryMobile>()
                };
            }

            return model;
        }

        private async Task<WM_Schedule> GetCurrentSchedule()
        {
            DateTime baseDate = DateTime.Now;
            var thisWeekStart = baseDate.AddDays(-(int)baseDate.DayOfWeek);
            var thisWeekEnd = thisWeekStart.AddDays(7).AddSeconds(-1);

            var timeStart = thisWeekStart.ToString("yyyy-MM-dd");
            var timeEnd = thisWeekEnd.AddDays(1).ToString("yyyy-MM-dd");

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'DateCreated': {");

            query.AppendLine(" '$gte': ISODate('" + timeStart + "T00:00:00.000+07:00') ");
            query.AppendLine(", '$lt': ISODate('" + timeEnd + "T00:00:00.000+07:00') ");

            query.AppendLine("}");

            query.AppendLine("}");

            var data = await _WM_ScheduleRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            return data.FirstOrDefault();
        }

        private async Task<List<WM_Diary>> GetDiariesByScheduleId_UserId(string scheduleid, string userid)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'UserId': { '$eq': '" + userid + "' }");
            query.AppendLine(", 'ScheduleId': { '$eq': '" + scheduleid + "' }");
            query.AppendLine("}");

            return await _WM_DiaryRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }
    }


}
