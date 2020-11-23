using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository
{
    public interface ISY_Map_Role_MenuRepository : IRepository<SY_Map_Role_Menu>
    {
    }

    public class SY_Map_Role_MenuRepository : Repository<SY_Map_Role_Menu>, ISY_Map_Role_MenuRepository
    {
        public SY_Map_Role_MenuRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}