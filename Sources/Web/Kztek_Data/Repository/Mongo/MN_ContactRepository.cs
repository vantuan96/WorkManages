using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.MN;

namespace Kztek_Data.Repository.Mongo
{
    public interface IMN_ContactRepository : IMongoRepository<MN_Contact>
    {
    }

    public class MN_ContactRepository : MongoRepository<MN_Contact>, IMN_ContactRepository
    {

    }
}
