using Kztek_Library.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kztek_Web.Components.Language
{
    public class MenuLanguageViewComponent : ViewComponent
    {
        public MenuLanguageViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string path, string menuname)
        {
            var text = await LanguageHelper.GetMenuLanguageText(path);
            text = !string.IsNullOrWhiteSpace(text) ? text : menuname;

            ViewBag.Text = text;
            return View();
        }
    }
}
