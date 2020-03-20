using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Jellypic.Web.Services
{
    public class Auth0TokenReader : IAuth0TokenReader
    {
        public async Task<JwtSecurityToken> ReadAsync(string idToken) =>
            (await ParseAsync(idToken)).SecurityToken;

        public async Task<string> ReaderUserIdAsync(string idToken) =>
            (await ParseAsync(idToken)).User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        async Task<(ClaimsPrincipal User, JwtSecurityToken SecurityToken)> ParseAsync(string idToken)
        {
            // sources:
            // https://stackoverflow.com/a/60776968/188740
            // and
            // https://www.jerriepelser.com/blog/manually-validating-rs256-jwt-dotnet/

            IConfigurationManager<OpenIdConnectConfiguration> configurationManager
                = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{ConfigSettings.Current.Auth0.Domain}.well-known/openid-configuration", new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);

            var validations = new TokenValidationParameters
            {
                ValidIssuer = ConfigSettings.Current.Auth0.Domain,
                ValidAudiences = new[] { ConfigSettings.Current.Auth0.ClientId },
                IssuerSigningKeys = openIdConfig.SigningKeys
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var user = tokenHandler.ValidateToken(idToken, validations, out var validatedToken);
            return (user, (JwtSecurityToken)validatedToken);
        }
    }
}
