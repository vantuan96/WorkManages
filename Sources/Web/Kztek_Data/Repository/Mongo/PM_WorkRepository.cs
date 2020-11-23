using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.PM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IPM_WorkRepository : IMongoRepository<PM_Work>
    {
    }

    public class PM_WorkRepository : MongoRepository<PM_Work>, IPM_WorkRepository
    {

    }
}
