using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.OneSignalr;

namespace Kztek_Data.Repository.Mongo
{
    public interface IOS_PlayerRepository : IMongoRepository<OS_Player>
    {
    }

    public class OS_PlayerRepository : MongoRepository<OS_Player>, IOS_PlayerRepository
    {

    }
}
