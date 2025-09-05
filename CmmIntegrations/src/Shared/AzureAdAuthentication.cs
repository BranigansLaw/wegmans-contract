using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Http
{
    public static class AzureAdAuthentication
    {        
        public static async Task<ClaimsPrincipal> ValidateAccessToken(this HttpRequest request, AuthorizationOptions adSettings)
        {
            _ = adSettings ?? throw new ArgumentNullException(nameof(adSettings));

            string accessToken;

            string[] parts = request.Headers?["Authorization"].ToString().Split(null) ?? new string[0];
            if (parts.Length == 2 && parts[0].Equals("Bearer"))
            {
                accessToken = parts[1];
            }
            else
            {
                throw new UnauthorizedAccessException("JWT not present");
            }            

            ConfigurationManager<OpenIdConnectConfiguration> configManager =
                new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{adSettings.Authority}.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());

            OpenIdConnectConfiguration config = null;
            config = await configManager.GetConfigurationAsync().ConfigureAwait(false);

            ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();

            // Initialize the token validation parameters
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                // App Id URI and AppId of this service application are both valid audiences.
                ValidAudiences = adSettings.Audience,

                // Support Azure AD V1 and V2 endpoints.
                ValidIssuers = adSettings.ValidIssuers,
                IssuerSigningKeys = config.SigningKeys
            };

            var claimsPrincipal = tokenValidator.ValidateToken(accessToken, validationParameters, out SecurityToken securityToken);
            return claimsPrincipal;
        }
    }
}
