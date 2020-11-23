using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Configs;
using Kztek_Library.Models;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Apis.Mobile
{
    [Authorize(Policy = ApiConfig.Auth_Bearer_Mobile)]
    [Route("api/mobile/[controller]")]
    public class ReportController : Controller
    {
        private IReportService _ReportService;

        public ReportController(IReportService _ReportService)
        {
            this._ReportService = _ReportService;
        }

        [HttpGet("personalperformance/{id}")]
        public async Task<Chart_Performance_Personal> personalperformance(string id)
        {
            return await _ReportService.ReportByUserId(id);
        }

        [HttpGet("personalperformancegrow/{id}")]
        public async Task<Chart_Performance_Grow_Personal> personalperformancegrow(string id)
        {
            return await _ReportService.ReportGrowByUserId(id);
        }
    }
}
