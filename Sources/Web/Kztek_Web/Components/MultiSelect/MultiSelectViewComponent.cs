using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kztek_Library.Helpers;
using Kztek_Library.Models;
using Kztek_Service.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kztek_Web.Components.ChosenSelect
{
    public class MultiSelectViewComponent : ViewComponent
    {
        private IHttpContextAccessor HttpContextAccessor;

        public MultiSelectViewComponent(IHttpContextAccessor HttpContextAccessor)
        {
            this.HttpContextAccessor = HttpContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(SelectListModel_Multi model)
        {
            return View(await Task.FromResult(model));
        }
    }
}