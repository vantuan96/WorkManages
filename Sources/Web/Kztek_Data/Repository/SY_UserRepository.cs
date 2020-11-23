using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository
{
    public interface ISY_UserRepository : IRepository<SY_User>
    {
    }

    public class SY_UserRepository : Repository<SY_User>, ISY_UserRepository
    {
        public SY_UserRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}