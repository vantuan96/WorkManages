using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository.Mongo
{
    public interface ISY_RoleRepository : IMongoRepository<SY_Role>
    {
    }

    public class SY_RoleRepository : MongoRepository<SY_Role>, ISY_RoleRepository
    {
        
    }
}