using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.PM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IPM_ComponentRepository : IMongoRepository<PM_Component>
    {
    }

    public class PM_ComponentRepository : MongoRepository<PM_Component>, IPM_ComponentRepository
    {

    }
}
