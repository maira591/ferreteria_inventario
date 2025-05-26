using Microsoft.AspNetCore.Mvc;

namespace Core.Application.Website.Providers
{
    public class AuthAttribute : TypeFilterAttribute
    {
        public AuthAttribute(params object[] privileges) : base(typeof(AuthorizeAction))
        {
            Arguments = new object[] {
                privileges
            };
        }
    }
}
