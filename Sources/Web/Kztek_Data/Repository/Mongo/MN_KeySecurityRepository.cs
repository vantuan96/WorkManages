using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.MN;

namespace Kztek_Data.Repository.Mongo
{
    public interface IMN_KeySecurityRepository : IMongoRepository<MN_KeySecurity>
    {
    }

    public class MN_KeySecurityRepository : MongoRepository<MN_KeySecurity>, IMN_KeySecurityRepository
    {

    }
}
