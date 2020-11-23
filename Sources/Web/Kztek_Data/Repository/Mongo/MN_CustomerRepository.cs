using System;
using Kztek_Data.Infrastructure;
using Kztek_Model.Models.MN;

namespace Kztek_Data.Repository.Mongo
{
    public interface IMN_CustomerRepository : IMongoRepository<MN_Customer>
    {
    }

    public class MN_CustomerRepository : MongoRepository<MN_Customer>, IMN_CustomerRepository
    {

    }
}
