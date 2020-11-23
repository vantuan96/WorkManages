using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.PM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IPM_ProjectRepository : IMongoRepository<PM_Project>
    {
    }

    public class PM_ProjectRepository : MongoRepository<PM_Project>, IPM_ProjectRepository
    {

    }
}
