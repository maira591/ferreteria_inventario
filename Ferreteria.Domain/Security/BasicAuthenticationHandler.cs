using Ferreteria.Domain.Utils;
using Ferreteria.Infrastructure.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Ferreteria.Domain.Security
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private static IConfigurator _configurationService = new ApplicationConfigurator();
        private readonly string _user;
        private readonly string _pass;
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration
            )
            : base(options, logger, encoder, clock)
        {
            _user = _configurationService.GetKey(ConstantsSecurity.user);
            _pass = _configurationService.GetKey(ConstantsSecurity.pass);
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // skip authentication if endpoint has [AllowAnonymous] attribute
            Endpoint endpoint = Context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return AuthenticateResult.NoResult();

            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Missing Authorization Header");

            UserSecurityModel user = null;
            try
            {
                AuthenticationHeaderValue authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                byte[] credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                string[] credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                string username = credentials[0];
                string password = credentials[1];
                user = new UserSecurityModel(username, password);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

            if (_user != user._user || _pass != user._pass)
                return AuthenticateResult.Fail("Invalid Username or Password");

            Claim[] claims = new[] {
                new Claim(ClaimTypes.Name, user._user),
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, Scheme.Name);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, Scheme.Name);

            return await Task.Run(() => AuthenticateResult.Success(ticket));
        }

    }
}
