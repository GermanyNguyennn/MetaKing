using IdentityServer4;
using IdentityServer4.Models;

namespace MetaKing.BackendServer.IdentityServer
{
    public static class Config
    {
        // Cấu hình các thông tin xác thực người dùng (claims)
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        // Cấu hình API Resource (không bắt buộc với IdentityServer4 >= 4, nhưng giữ lại nếu cần)
        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api.metaking", "MetaKing API")
                {
                    Scopes = { "api.metaking" }
                }
            };

        // Cấu hình API Scopes
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("api.metaking", "MetaKing API Scope")
            };

        // Cấu hình các ứng dụng (clients) được phép kết nối đến IdentityServer
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // WebPortal sử dụng code flow (hybrid flow nếu có client secret)
                new Client
                {
                    ClientId = "webportal",
                    ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,
                    AllowOfflineAccess = true,

                    RedirectUris = { "https://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "api.metaking"
                    }
                },

                // Swagger UI (không cần client secret, PKCE bắt buộc)
                new Client
                {
                    ClientId = "swagger",
                    ClientName = "Swagger UI",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,

                    RedirectUris = { "https://localhost:5000/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "https://localhost:5000/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = { "https://localhost:5000" },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api.metaking"
                    }
                },

                // Angular Client
                new Client
                {
                    ClientId = "angular_admin",
                    ClientName = "Angular Admin",
                    RequireClientSecret = false,
                    RequirePkce = true,
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference,

                    RedirectUris = new List<string>
                    {
                        "http://localhost:4200",
                        "http://localhost:4200/authentication/login-callback",
                        "http://localhost:4200/silent-renew.html"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:4200/unauthorized",
                        "http://localhost:4200/authentication/logout-callback",
                        "http://localhost:4200"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "http://localhost:4200"
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api.metaking"
                    }
                }
            };
    }
}
