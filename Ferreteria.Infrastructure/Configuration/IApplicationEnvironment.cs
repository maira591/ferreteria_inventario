using InfrastructureUserBasic = Ferreteria.Infrastructure.Model.UserBasic;

namespace Ferreteria.Infrastructure.Configuration
{
    public interface IApplicationEnvironment
    {
        string GetConnectionString();
        string GetCurrentUser();
        InfrastructureUserBasic GetCurrentUserBasic();
        string GetIp();
        string GetDefaultSchema();
    }
}
