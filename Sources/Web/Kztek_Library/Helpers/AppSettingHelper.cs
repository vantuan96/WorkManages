using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Kztek_Library.Helpers
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

        public static Task<string> GetStringFromFileJson(string file, string key)
        {
            var valu = "";

            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), file + ".json");
            configurationBuilder.AddJsonFile(path, false);

            var root = configurationBuilder.Build();
            valu = root[key];

            return Task.FromResult(valu);
        }
    }
}