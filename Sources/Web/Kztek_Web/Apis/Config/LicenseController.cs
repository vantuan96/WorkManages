using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Configs;
using Kztek_Model.Models.MN;
using Kztek_Service.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Kztek_Web.Apis.Config
{
    //[Authorize(Policy = ApiConfig.Auth_Bearer_Config)]
    [Route("api/config/[controller]")]
    public class LicenseController : Controller
    {
        private ILicenseService _LicenseService;

        public LicenseController(ILicenseService _LicenseService)
        {
            this._LicenseService = _LicenseService;
        }

        [AllowAnonymous]
        [HttpGet("{name}")]
        public async Task<MN_License> license(string name)
        {
            return await _LicenseService.License(name);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<List<MN_License>> getall()
        {
            return await _LicenseService.GetAll();
        }
    }
}
