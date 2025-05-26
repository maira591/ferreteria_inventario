using System.Configuration;

namespace Ferreteria.Infrastructure.Configuration
{
    public class ApplicationConfigurator : IConfigurator
    {
        public ApplicationConfigurator()
        {
        }

        public string GetKey(string key)
        {
            string item = ConfigurationManager.AppSettings[key];
            if (item == null)
            {
                return string.Empty;
            }
            return item;
        }
    }
}
