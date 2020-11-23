using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository.Mongo
{
    public interface ISY_Map_Role_MenuRepository : IMongoRepository<SY_Map_Role_Menu>
    {
    }

    public class SY_Map_Role_MenuRepository : MongoRepository<SY_Map_Role_Menu>, ISY_Map_Role_MenuRepository
    {
        
    }
}