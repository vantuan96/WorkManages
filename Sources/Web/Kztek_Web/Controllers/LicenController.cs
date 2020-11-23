using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Service.Admin.Interfaces.MN;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Controllers
{
    public class LicenController : Controller
    {
        #region DI
        private IMN_LicenseService _MN_LicenseService;
        public LicenController (IMN_LicenseService _MN_LicenseService)
        {
            this._MN_LicenseService = _MN_LicenseService;
        }
        #endregion
        public async Task<IActionResult> Index(string key = "" , string fromdate="" , string todate ="" ,int page= 1)
        {
            int pagesize = 10;

            var gridModel = await _MN_LicenseService.GetPagings(key, fromdate, todate, page, pagesize);
            ViewBag.key = key;
            ViewBag.Fromdate = fromdate;
            ViewBag.Todate = !string.IsNullOrWhiteSpace(todate) ? Convert.ToDateTime(todate).ToString("dd/MM/yyyy HH:mm:59") : DateTime.Now.ToString("dd/MM/yyyy 23:59:59");
            return View(gridModel);
            ///
        }
    }
}
