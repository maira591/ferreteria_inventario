using Core.Application.Website.Models;
using Core.Application.Website.Utils;
using Core.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.ServiceCollection
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
