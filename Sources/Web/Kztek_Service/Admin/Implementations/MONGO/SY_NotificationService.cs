using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Model.Models;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Service.Admin.Implementations.MONGO
{
    public class SY_NotificationService : ISY_NotificationService
    {
        private ISY_NotificationRepository _SY_NotificationRepository;

        public SY_NotificationService(ISY_NotificationRepository _SY_NotificationRepository)
        {
            this._SY_NotificationRepository = _SY_NotificationRepository;
        }

        public async Task<MessageReport> Create(SY_Notification model)
        {
            return await _SY_NotificationRepository.Add(model);
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

                return await _SY_NotificationRepository.Remove(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
            }
            else
            {
                result = new MessageReport(false, "Bản ghi không tồn tại");
            }

            return await Task.FromResult(result);
        }

        public async Task<List<SY_Notification>> GetAll()
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("}");

            return await _SY_NotificationRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));
        }

        public async Task<List<SY_Notification>> GetAllOrder()
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("}");

            var data = await _SY_NotificationRepository.GetManyToList(MongoHelper.ConvertQueryStringToDocument(query.ToString()));

            var cus = data.OrderByDescending(n => n.DateCreated).Take(10).ToList();

            return cus;
        }

        public async Task<SY_Notification> GetById(string id)
        {
            return await _SY_NotificationRepository.GetOneById(id);
        }

        public async Task<MessageReport> Update(SY_Notification model)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("'_id': { '$eq': '" + model.Id + "' }");
            query.AppendLine("}");

            return await _SY_NotificationRepository.Update(MongoHelper.ConvertQueryStringToDocument(query.ToString()), model);
        }
    }
}
