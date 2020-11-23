using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kztek_Web.Models;
using Kztek_Library.Models;
using Kztek_Web.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Kztek_Service.Admin.Interfaces;

namespace Kztek_Web.Controllers
{
    public class HomeController : Controller
    {
        private ISY_MenuFunctionService _SY_MenuFunctionService;

        public HomeController(ISY_MenuFunctionService _SY_MenuFunctionService)
        {
            this._SY_MenuFunctionService = _SY_MenuFunctionService;
        }

        [CheckSessionCookie]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NotAuthorize() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult NoAuthorized()
        {
            return View();
        }
    }
}
