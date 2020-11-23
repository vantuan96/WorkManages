using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NC_WorkmanagementSelfHost.Models;
using NC_WorkmanagementSelfHost.Statics;

namespace NC_WorkmanagementSelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            ScheduleService.SyncData();

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
            webBuilder.UseUrls(AppSettings.Host);
        })
            .UseWindowsService();
    }
}
