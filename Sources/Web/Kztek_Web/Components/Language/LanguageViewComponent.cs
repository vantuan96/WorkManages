using Kztek_Library.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kztek_Web.Components.Language
{
    public class LanguageViewComponent : ViewComponent
    {
        public LanguageViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string path)
        {
            var text = await LanguageHelper.GetLanguageText(path);
            ViewBag.Text = text;
            return View();
        }
    }
}
