using Kztek_Data.Infrastructure;
using Kztek_Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Kztek_Data.Repository
{
    public interface ISY_MenuFunctionRepository : IRepository<SY_MenuFunction>
    {
    }

    public class SY_MenuFunctionRepository : Repository<SY_MenuFunction>, ISY_MenuFunctionRepository
    {
        public SY_MenuFunctionRepository(DbContextOptions<Kztek_Entities> options) : base(options)
        {
        }
    }
}