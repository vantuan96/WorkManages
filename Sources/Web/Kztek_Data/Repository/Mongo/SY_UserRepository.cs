using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository.Mongo
{
    public interface ISY_UserRepository : IMongoRepository<SY_User>
    {
    }

    public class SY_UserRepository : MongoRepository<SY_User>, ISY_UserRepository
    {
       
    }
}