using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository.Mongo
{
    public interface ISY_Map_User_RoleRepository : IMongoRepository<SY_Map_User_Role>
    {
    }

    public class SY_Map_User_RoleRepository : MongoRepository<SY_Map_User_Role>, ISY_Map_User_RoleRepository
    {
        
    }
}