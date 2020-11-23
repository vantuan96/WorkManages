using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository.Mongo
{
    public interface ISY_MenuFunctionRepository : IMongoRepository<SY_MenuFunction>
    {
    }

    public class SY_MenuFunctionRepository : MongoRepository<SY_MenuFunction>, ISY_MenuFunctionRepository
    {
       
    }
}