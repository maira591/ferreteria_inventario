using Ferreteria.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;

namespace Ferreteria.Infrastructure.ServiceCollection
{
    public static class DataBaseContext
    {
        public static void AddDBContext(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<FerreteriaContext>(options =>
                options.UseMySQL(connectionString));
        }
    }
}
