using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Repository.Mongo;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Api.Interfaces;

namespace Kztek_Service.Api.Implementations.MONGO
{
    public class NotificationService : INotificationService
    {
        private ISY_NotificationRepository _SY_NotificationRepository;

        public NotificationService(ISY_NotificationRepository _SY_NotificationRepository)
        {
            this._SY_NotificationRepository = _SY_NotificationRepository;
        }

        public async Task<GridModel<SY_NotificationCustomView>> GetPagingByFirst(int pageNumber, int pageSize)
        {
            var query = new StringBuilder();
            query.AppendLine("{");
            query.AppendLine("}");

            var sort = new StringBuilder();
            sort.AppendLine("{");
            sort.AppendLine("'DateCreated': -1");
            sort.AppendLine("}");

            var data = await _SY_NotificationRepository.GetPaging(MongoHelper.ConvertQueryStringToDocument(query.ToString()), MongoHelper.ConvertQueryStringToDocument(sort.ToString()), pageNumber, pageSize);

            var cus = new List<SY_NotificationCustomView>();

            foreach (var item in data.Data)
            {
                cus.Add(new SY_NotificationCustomView() {
                    dateCreated = item.DateCreated.ToString("dd/MM/yyyy HH:mm"),
                    description = item.Description,
                    id = item.Id,
                    title = item.Title
                });
            }

            var k = new GridModel<SY_NotificationCustomView>() {
                Data = cus,
                PageIndex = pageNumber,
                PageSize = pageSize,
                TotalIem = data.TotalIem,
                TotalPage = data.TotalPage
            };

            return k;
        }
    }
}