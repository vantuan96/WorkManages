using Kztek_Data.Infrastructure;
using Kztek_Model.Models.WM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IWM_ScheduleRepository : IMongoRepository<WM_Schedule>
    {
    }

    public class WM_ScheduleRepository : MongoRepository<WM_Schedule>, IWM_ScheduleRepository
    {

    }
}