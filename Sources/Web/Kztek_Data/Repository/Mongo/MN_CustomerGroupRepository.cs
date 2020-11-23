using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.MN;

namespace Kztek_Data.Repository.Mongo
{
    public interface IMN_CustomerGroupRepository : IMongoRepository<MN_CustomerGroup>
    {
    }

    public class MN_CustomerGroupRepository : MongoRepository<MN_CustomerGroup>, IMN_CustomerGroupRepository
    {

    }
}
