using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kztek_Data.Repository.Mongo
{
    public interface ISY_ReminderSystemRepository : IMongoRepository<SY_ReminderSystem>
    {
    }

    public class SY_ReminderSystemRepository : MongoRepository<SY_ReminderSystem>, ISY_ReminderSystemRepository
    {

    }
}
