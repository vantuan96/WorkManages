using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NC_WorkmanagementSelfHost.Helpers
{
    public class AppSettingHelper
    {
        public static Task<string> GetStringFromAppSetting(string key)
        {
            var valu = "";

            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            valu = root[key];

            return Task.FromResult(valu);
        }
    }
}
