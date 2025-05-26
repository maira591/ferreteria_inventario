using Ferreteria.DataAccess.Core;
using Ferreteria.DataAccess.EF;
using Ferreteria.Domain.LoginService;
using Ferreteria.Domain.LogService;
using Ferreteria.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ferreteria.Infrastructure.ServiceCollection
{
    public static class IoC
    {
        public static void AddDependency(IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IConfigurator, ApplicationConfigurator>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILogService, LogService>();
        }
    }
}
