using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Service.Admin.Implementations.MONGO
{
    public class SY_ReminderSystemService : ISY_ReminderSystemService
    {
        private ISY_ReminderSystemRepository _SY_ReminderSystemRepository;

        public SY_ReminderSystemService(ISY_ReminderSystemRepository _SY_ReminderSystemRepository)
        {
            this._SY_ReminderSystemRepository = _SY_ReminderSystemRepository;
        }

        public async Task<MessageReport> Create(SY_ReminderSystem model)
        {
            return await _SY_ReminderSystemRepository.Add(model);
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

                return await _SY_ReminderSystemRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<MessageReport> EditData(string recordid, string recordtype, string datereminder, bool isdone)
        {
            //Kiểm tra đã có record chưa
            var obj = await GetById(recordid);
            if (obj == null)
            {
                obj = new SY_ReminderSystem()
                {
                    Id = recordid,
                    Type = recordtype,
                    DateReminder = Convert.ToDateTime(datereminder),
                    isDone = isdone,
                };

                return await Create(obj);
            }
            else
            {
                obj.DateReminder = Convert.ToDateTime(datereminder);
                obj.isDone = isdone;

                return await Update(obj);
            }
        }

        public async Task<SY_ReminderSystem> GetById(string id)
        {
            return await _SY_ReminderSystemRepository.GetOneById(id);
        }

        public async Task<MessageReport> Update(SY_ReminderSystem model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _SY_ReminderSystemRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
