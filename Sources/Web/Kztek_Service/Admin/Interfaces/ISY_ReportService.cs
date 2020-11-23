using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Kztek_Library.Models;

namespace Kztek_Service.Admin.Interfaces
{
    public interface ISY_ReportService
    {
        Task<List<Chart_Performance_Monthly>> Users_Performance(List<string> userids);

        Task<List<Chart_Performance_Team>> Team_Performance(List<string> userids);

        Task<List<Chart_Performance_Grow>> Users_PerformanceGrow(List<string> userids);
    }
}
