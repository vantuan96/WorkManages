using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models.WM;
using Kztek_Service.Admin.Interfaces.WM;

namespace Kztek_Service.Admin.Implementations.MONGO.WM
{
    public class WM_ScheduleService : IWM_ScheduleService
    {
        private IWM_ScheduleRepository _WM_ScheduleRepository;

        public WM_ScheduleService(IWM_ScheduleRepository _WM_ScheduleRepository)
        {
            this._WM_ScheduleRepository = _WM_ScheduleRepository;
        }

        public async Task<MessageReport> Create(WM_Schedule model)
        {
            return await _WM_ScheduleRepository.Add(model);
        }

        public async Task<MessageReport> Delete(string id)
        {
            var result = new MessageReport(false, "Có lỗi xảy ra");

            var obj = await GetById(id);
            if (obj != null)
            {
                var query = new StringBuilder();
                query.AppendLine("{");
                query.AppendLine("'_id': { '$eq': '" + id + "' }");
                query.AppendLine("}");

                return await _WM_ScheduleRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<WM_Schedule> GetById(string id)
        {
            return await _WM_ScheduleRepository.GetOneById(id);
        }

        public async Task<List<WM_Schedule>> GetCurrentWeekSchedule(DateTime DateStart, DateTime DateEnd)
        {
            var timeStart = DateStart.ToString("yyyy-MM-dd");
            var timeEnd = DateEnd.AddDays(1).ToString("yyyy-MM-dd");

            var query = new StringBuilder();
            query.AppendLine("{");

            query.AppendLine("'DateCreated': {");

            query.AppendLine(" '$gte': ISODate('" + timeStart + "T00:00:00.000+07:00') ");
            query.AppendLine(", '$lt': ISODate('" + timeEnd + "T00:00:00.000+07:00') ");

            query.AppendLine("}");

            query.AppendLine("}");

            return await _WM_ScheduleRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<WM_Schedule_Submit> GetCustomById(string id)
        {
            var model = new WM_Schedule_Submit();

            var obj = await _WM_ScheduleRepository.GetOneById(id);
            if (obj != null)
            {
                model = await GetCustomByModel(obj);
            }

            return model;
        }

        public async Task<WM_Schedule_Submit> GetCustomByModel(WM_Schedule model)
        {
            var obj = new WM_Schedule_Submit()
            {
                Id = model.Id,
                DateEnd = model.DateEnd.ToString("dd/MM/yyyy HH:mm"),
                DateStart = model.DateStart.ToString("dd/MM/yyyy HH:mm"),
                Description = model.Description,
                Title = model.Title
            };

            return await Task.FromResult(obj);
        }

        public async Task<GridModel<WM_Schedule>> GetPaging(string key, int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");

            if (!string.IsNullOrWhiteSpace(key))
            {
                query.AppendLine("'$or': [");

                query.AppendLine("{ 'Title': { '$in': [/" + key + "/i] } }");
                query.AppendLine(", { 'Description': { '$in': [/" + key + "/i] } }");

                query.AppendLine("]");
            }

            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'DateCreated': -1");
            sort.AppendLine("}");

            return await _WM_ScheduleRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);
        }

        public async Task<MessageReport> Update(WM_Schedule model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _WM_ScheduleRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}