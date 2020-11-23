using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Model.Models;

namespace Kztek_Service.Admin.Interfaces
{
    public interface ISY_NotificationService
    {
        Task<List<SY_Notification>> GetAll();

        Task<List<SY_Notification>> GetAllOrder();

        Task<SY_Notification> GetById(string id);

        Task<MessageReport> Create(SY_Notification model);

        Task<MessageReport> Update(SY_Notification model);

        Task<MessageReport> Delete(string id);
    }
}
