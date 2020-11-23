using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository
{
    public interface ISY_Map_User_RoleRepository : IRepository<SY_Map_User_Role>
    {
    }

    public class SY_Map_User_RoleRepository : Repository<SY_Map_User_Role>, ISY_Map_User_RoleRepository
    {
        public SY_Map_User_RoleRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}