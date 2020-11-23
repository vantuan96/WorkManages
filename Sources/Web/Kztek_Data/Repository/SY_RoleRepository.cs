using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository
{
    public interface ISY_RoleRepository : IRepository<SY_Role>
    {
    }

    public class SY_RoleRepository : Repository<SY_Role>, ISY_RoleRepository
    {
        public SY_RoleRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}