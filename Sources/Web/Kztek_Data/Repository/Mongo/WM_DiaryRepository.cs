using Kztek_Data.Infrastructure;
using Kztek_Model.Models.WM;

namespace Kztek_Data.Repository.Mongo
{
    public interface IWM_DiaryRepository : IMongoRepository<WM_Diary>
    {
    }

    public class WM_DiaryRepository : MongoRepository<WM_Diary>, IWM_DiaryRepository
    {

    }
}