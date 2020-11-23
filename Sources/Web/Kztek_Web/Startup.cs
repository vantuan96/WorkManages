using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Kztek.Security;
using Kztek_Core.Models;
using Kztek_Data;
using Kztek_Data.Repository;
using Kztek_Library.Configs;
using Kztek_Library.Helpers;
using Kztek_Service.Admin;
using Kztek_Web.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;

namespace Kztek_Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("vi-VN");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("vi-VN") };
            });


            services.AddMemoryCache();
            services.AddMvc(option => option.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
                //hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(30);
            });

            services.AddControllersWithViews()
        .AddNewtonsoftJson();
            services.AddRazorPages();

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                // Use the default property (Pascal) casing
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            //Cấu hình bảo mật API
            services.AddAuthentication()
            .AddJwtBearer(ApiConfig.Auth_Bearer_Mobile, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer_Mobile"],
                    ValidAudience = Configuration["Jwt:Issuer_Mobile"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityModel.Mobile_Key))
                };
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(ApiConfig.Auth_Bearer_Mobile)
                .Build();

                options.AddPolicy(ApiConfig.Auth_Bearer_Mobile, policy =>
                {
                    policy.AuthenticationSchemes.Add(ApiConfig.Auth_Bearer_Mobile);
                    policy.RequireAuthenticatedUser();
                });
            });

            //Chuyển db
            var connect = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultConnection").Result;
            var connecttype = AppSettingHelper.GetStringFromFileJson("connectstring", "ConnectionStrings:DefaultType").Result;

            switch (connecttype)
            {

                case DatabaseModel.SQLSERVER:

                    services.AddDbContext<Kztek_Entities>(opts => opts.UseSqlServer(connect));

                    break;

                case DatabaseModel.MYSQL:

                    services.AddDbContext<Kztek_Entities>(opts => opts.UseMySQL(connect));

                    break;

                default:

                    services.AddDbContext<Kztek_Entities>(opts => opts.UseSqlServer(connect));

                    break;
            }

            //services.AddDbContext<Kztek_Entities>(opts => opts.UseMySQL(Configuration.GetConnectionString("DefaultConnection")));

            services.AddSession();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // //SY_User
            // services.AddScoped<ISY_UserRepository, SY_UserRepository>();
            // services.AddScoped<ISY_UserService, SY_UserService>();

            // //SY_Role
            // services.AddScoped<ISY_RoleRepository, SY_RoleRepository>();
            // services.AddScoped<ISY_RoleService, SY_RoleService>();

            // //SY_MenuFunction
            // services.AddScoped<ISY_MenuFunctionRepository, SY_MenuFunctionRepository>();
            // services.AddScoped<ISY_MenuFunctionService, SY_MenuFunctionService>();

            // //Map
            // services.AddScoped<ISY_Map_User_RoleRepository, SY_Map_User_RoleRepository>();
            // services.AddScoped<ISY_Map_Role_MenuRepository, SY_Map_Role_MenuRepository>();

            services.AddSingleton<CacheHelper>();

            //

            var builder = new ContainerBuilder();

            switch (connecttype)
            {
                case DatabaseModel.MONGO:

                    builder.RegisterAssemblyTypes(typeof(Kztek_Data.Repository.Mongo.SY_UserRepository).Assembly)
                    .Where(t => t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces().InstancePerLifetimeScope();

                    break;

                default:

                    builder.RegisterAssemblyTypes(typeof(SY_UserRepository).Assembly)
                    .Where(t => t.Name.EndsWith("Repository"))
                    .AsImplementedInterfaces().InstancePerLifetimeScope();

                    break;
            }


            //Mapping service theo đúng cơ sở dữ liệu
            switch (connecttype)
            {
                case DatabaseModel.SQLSERVER:

                    builder.RegisterAssemblyTypes(typeof(Kztek_Service.Admin.Implementations.SQLSERVER.SY_UserService).Assembly)
                    .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains(DatabaseModel.SQLSERVER))
                    .AsImplementedInterfaces().InstancePerLifetimeScope();

                    break;

                case DatabaseModel.MYSQL:

                    builder.RegisterAssemblyTypes(typeof(Kztek_Service.Admin.Implementations.MYSQL.SY_UserService).Assembly)
                     .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains(DatabaseModel.MYSQL))
                     .AsImplementedInterfaces().InstancePerLifetimeScope();

                    break;

                case DatabaseModel.MONGO:

                    builder.RegisterAssemblyTypes(typeof(Kztek_Service.Admin.Implementations.MONGO.SY_UserService).Assembly)
                     .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains(DatabaseModel.MONGO))
                     .AsImplementedInterfaces().InstancePerLifetimeScope();

                    break;

                default:

                    builder.RegisterAssemblyTypes(typeof(Kztek_Service.Admin.Implementations.SQLSERVER.SY_UserService).Assembly)
                     .Where(t => t.Name.EndsWith("Service") && t.Namespace.Contains(DatabaseModel.SQLSERVER))
                     .AsImplementedInterfaces().InstancePerLifetimeScope();

                    break;
            }

            builder.Populate(services);

            var container = builder.Build();

            //Create the IServiceProvider based on the container.
            return new AutofacServiceProvider(container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("vi-VN"),
                // Formatting numbers, dates, etc.
                SupportedCultures = new List<CultureInfo> { new CultureInfo("vi-VN") },
                // UI strings that we have localized.
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("vi-VN") }
            });

            app.UseDeveloperExceptionPage();

            //if (env.IsDevelopment())
            //{

            //}
            //else
            //{
            //    app.UseExceptionHandler("/Home/Error");
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}
            //app.UseMvcWithDefaultRoute();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            // app.UseEndpoints(endpoints =>
            // {
            //    endpoints.MapHub<WorkHub>("/workHub");
            // });

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=Home}/{action=Index}/{id?}");
            //});
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapHub<WorkHub>("/workHub");
            });

            app.UseCookiePolicy(); 
        }
    }
}
