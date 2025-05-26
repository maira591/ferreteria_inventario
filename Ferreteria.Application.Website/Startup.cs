using AutoMapper;
using Core.Application.Website.Mapping;
using Core.Application.Website.Utils;
using Core.Infrastructure.Mapping;
using Core.Infrastructure.ServiceCollection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Configuration;
using System.Globalization;

namespace Core.Application.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddAutoMapper(typeof(Startup));
            services.AddRazorPages();
            services.AddHttpContextAccessor();

            IoC.AddDependency(services);
            IoCWeb.AddDependency(services);

            DataBaseContext.AddDBContext(services);

            //mapper
            var mapperConfig = new MapperConfiguration(m =>
            {
                m.AddProfile(new MappingProfile());
                m.AddProfile(new MappingProfileWeb());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddMvc(options =>
            {
                options.Filters.Add(new AllowAnonymousFilter());
                options.EnableEndpointRouting = true;

            }).AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);



            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
               .AddCookie((o) =>
               {
                   o.Cookie.HttpOnly = true;
                   o.LoginPath = "/Login";
                   o.AccessDeniedPath = string.Empty;
               });

            //Modificacion de vistas en tiempo de ejecución.
            services.AddControllersWithViews().AddRazorRuntimeCompilation();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(int.Parse(ConfigurationManager.AppSettings["timeoutSessionInMinutes"]));
            });

            services.AddSignalR();
            services.AddMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var cultureInfo = new CultureInfo("es-CO");
            cultureInfo.NumberFormat.CurrencySymbol = "$";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }



            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/Error/HandleError/404";
                    app.UseStatusCodePages();
                    app.UseHsts();
                    app.UseStatusCodePagesWithRedirects("/Error/HandleError/404");
                    await next();
                }
            });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}");

                endpoints.MapHub<FormatValidationsHub>("/receive");
            });
        }
    }
}
