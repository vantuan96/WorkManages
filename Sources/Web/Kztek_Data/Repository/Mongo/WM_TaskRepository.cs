using Kztek_Data.Infrastructure;
using Kztek_Model.Models.WM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IWM_TaskRepository : IMongoRepository<WM_Task>
    {
    }

    public class WM_TaskRepository : MongoRepository<WM_Task>, IWM_TaskRepository
    {

    }
}