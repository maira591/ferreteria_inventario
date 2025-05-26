using AutoMapper;
using Ferreteria.Application.Website.Mapping;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Infrastructure.Mapping;
using Ferreteria.Infrastructure.ServiceCollection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios (ConfigureServices)

// Agrega controladores con vistas y filtros
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AllowAnonymousFilter());
    options.EnableEndpointRouting = true;
}).AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
  .AddRazorRuntimeCompilation();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRazorPages();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

IoC.AddDependency(builder.Services);
IoCWeb.AddDependency(builder.Services);
DataBaseContext.AddDBContext(builder.Services, builder.Configuration);

// AutoMapper manual
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AddProfile(new MappingProfile());
    cfg.AddProfile(new MappingProfileWeb());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

// Autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.Cookie.HttpOnly = true;
        o.LoginPath = "/Login";
        o.AccessDeniedPath = string.Empty;
    });

// Configuración de sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(
        int.Parse(System.Configuration.ConfigurationManager.AppSettings["timeoutSessionInMinutes"] ?? "30") 
    );
});

var app = builder.Build();

// Configuración del middleware (Configure)

// Cultura por defecto
var cultureInfo = new CultureInfo("es-CO");
cultureInfo.NumberFormat.CurrencySymbol = "$";
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

// Manejo de errores
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Middleware para manejar errores 404
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Error/HandleError/404";
        app.UseStatusCodePagesWithRedirects("/Error/HandleError/404");
        await next();
    }
});

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}");

app.MapHub<FormatValidationsHub>("/receive");

app.Run();
