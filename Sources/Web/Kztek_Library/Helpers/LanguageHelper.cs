using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kztek_Library.Helpers
{
    public class LanguageHelper
    {
        public static async Task<string> GetLanguageText(string path)
        {
            var region = await AppSettingHelper.GetStringFromAppSetting("Languages");
            path = $"{path}:{region}";
            var text = await AppSettingHelper.GetStringFromFileJson("Languages/languages", path);

            text = string.IsNullOrWhiteSpace(text) ? "" : text;

            return text;
        }

        public static async Task<string> GetMenuLanguageText(string path)
        {
            var region = await AppSettingHelper.GetStringFromAppSetting("Languages");
            path = $"{path}:{region}";
            var text = await AppSettingHelper.GetStringFromFileJson("Languages/menu-languages", path);

            text = string.IsNullOrWhiteSpace(text) ? "" : text;

            return text;
        }
    }
}
