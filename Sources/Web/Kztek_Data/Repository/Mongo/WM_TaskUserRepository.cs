using Kztek_Data.Infrastructure;
using Kztek_Model.Models.WM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IWM_TaskUserRepository : IMongoRepository<WM_TaskUser>
    {
    }

    public class WM_TaskUserRepository : MongoRepository<WM_TaskUser>, IWM_TaskUserRepository
    {

    }
}