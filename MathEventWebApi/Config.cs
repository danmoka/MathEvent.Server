using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace MathEventWebApi
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api", "MathEvent API")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    ClientSecrets = { new Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "api" }
                },
                new Client {
                    ClientId = "react_spa",
                    ClientName = "React SPA",
                    ClientSecrets = { new Secret("secret".Sha512()) },
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    RedirectUris = {"http://localhost:5000/auth-callback"},
                    PostLogoutRedirectUris = {"http://localhost:5000/"},
                    AllowedCorsOrigins = {"http://localhost:5000"},
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600,
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api"
                    }
                }
            };
    }
}