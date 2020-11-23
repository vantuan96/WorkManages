using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models;

namespace Kztek_Data.Repository.Mongo
{
    public interface ISY_NotificationRepository : IMongoRepository<SY_Notification>
    {
    }

    public class SY_NotificationRepository : MongoRepository<SY_Notification>, ISY_NotificationRepository
    {

    }
}
