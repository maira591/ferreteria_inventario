using Ferreteria.Application.Website.Models;
using Ferreteria.Application.Website.Utils;
using Ferreteria.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ferreteria.Infrastructure.ServiceCollection
{
    public static class IoCWeb
    {
        public static void AddDependency(IServiceCollection services)
        {
            services.AddScoped<IUserBasicModel, UserBasicModel>();
            services.AddScoped<IApplicationEnvironment, ApplicationEnvironment>();
        }
    }
}
