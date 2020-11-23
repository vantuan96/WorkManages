using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.MN;

namespace Kztek_Data.Repository.Mongo
{
    public interface IMN_KeyCardRepository : IMongoRepository<MN_KeyCard>
    {
    }

    public class MN_KeyCardRepository : MongoRepository<MN_KeyCard>, IMN_KeyCardRepository
    {

    }
}
