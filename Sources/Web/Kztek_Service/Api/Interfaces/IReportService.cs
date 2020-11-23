using System;
using System.Threading.Tasks;
using Kztek_Library.Models;

namespace Kztek_Service.Api.Interfaces
{
    public interface IReportService
    {
        Task<Chart_Performance_Personal> ReportByUserId(string userid);

        Task<Chart_Performance_Grow_Personal> ReportGrowByUserId(string userid);
    }
}
