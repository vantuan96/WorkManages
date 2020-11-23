using System;
using System.Threading.Tasks;
using Kztek_Core.Models;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.MN;
using MongoDB.Bson;

namespace Kztek_Data.Repository.Mongo
{
    public interface IMN_LicenseRepository : IMongoRepository<MN_License>
    {
       
    }

    public class MN_LicenseRepository : MongoRepository<MN_License>, IMN_LicenseRepository
    {

    }
}
