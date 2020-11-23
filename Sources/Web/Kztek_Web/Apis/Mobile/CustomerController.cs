using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Core.Models;
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
    public class CustomerController : Controller
    {
        private ICustomerService _CustomerService;

        public CustomerController(ICustomerService _CustomerService)
        {
            this._CustomerService = _CustomerService;
        }

        [HttpGet("getpagingbyfirst")]
        public async Task<GridModel<MN_CustomerCustomView>> getpagingbyfirst(string key, int pageIndex, int pageSize)
        {
            return await _CustomerService.GetPagingByFirst(key, pageIndex, pageSize);
        }

        [HttpGet("getcustomerdetail/{id}")]
        public async Task<MN_CustomerDetailCustomView> getcustomerdetail(string id)
        {
            return await _CustomerService.GetCustomById(id);
        }
    }
}
