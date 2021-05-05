using System.Collections.Generic;
using IdentityServer4;
using IdentityServer4.Models;
using ProjectX.Identity.Domain;

namespace ProjectX.Identity.Persistence.IdentityServer
{
    internal static class IdentityServerConfig
    {
        public static class Scopes
        {
            public const string Identity = "identity";
            public const string Internal = "internal";
            public const string Realtime = "realtime";
            public const string Blog = "blog";
            public const string Messenger = "messenger";
            public const string FileStorage = "storage";
        }

        public static class Clients
        {
            public const string WebClient = "webclient";
            public const string SwaggerClient = "swagger";
            public const string InternalClient = "internal";
        }

        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address()
            };

        public static IEnumerable<ApiResource> GetApis() =>
            new List<ApiResource>
            {
                new ApiResource(Scopes.Identity, "API Identity")
                {
                    ApiSecrets = new List<Secret> { new Secret("identity-api".Sha256(), "identity-api") }
                },
                new ApiResource(Scopes.Blog, "Api blog"),
                new ApiResource(Scopes.Internal, "Internal"),
                new ApiResource(Scopes.Realtime, "Api realtime"),
                new ApiResource(Scopes.Messenger, "Api messenger"),
                new ApiResource(Scopes.FileStorage, "Api file storage")
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
                new Client
                {
                    ClientId = Clients.WebClient,
                    ClientName = Clients.WebClient,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = SessionLifetime.AccessTokenLifetime,
                    SlidingRefreshTokenLifetime = SessionLifetime.RefreshTokenLifetime,
                    //when refreshing the token, the lifetime of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime).
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    //the refresh token handle will be updated when refreshing tokens
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    ClientSecrets =
                    {
                        new Secret("webclientSecret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        Scopes.Identity,
                        Scopes.Blog,
                        Scopes.Realtime,
                        Scopes.Messenger,
                        Scopes.FileStorage
                    }
                },

                new Client
                {
                    ClientId = Clients.SwaggerClient,
                    ClientName = Clients.SwaggerClient,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowOfflineAccess = true,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AccessTokenLifetime = SessionLifetime.AccessTokenLifetime,
                    SlidingRefreshTokenLifetime = SessionLifetime.RefreshTokenLifetime,
                    //when refreshing the token, the lifetime of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime).
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    //the refresh token handle will be updated when refreshing tokens
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    ClientSecrets =
                    {
                        new Secret("swaggerSecret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        Scopes.Identity,
                        Scopes.Realtime,
                        Scopes.Internal,
                        Scopes.Blog,
                        Scopes.Messenger,
                        Scopes.FileStorage
                    }
                },

                new Client
                {
                    ClientId = Clients.InternalClient,
                    ClientName = Clients.InternalClient,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    RequireClientSecret = false,
                    AllowOfflineAccess = true,
                    AccessTokenLifetime = SessionLifetime.AccessTokenLifetime,
                    SlidingRefreshTokenLifetime = SessionLifetime.RefreshTokenLifetime,
                    //when refreshing the token, the lifetime of the refresh token will be renewed (by the amount specified in SlidingRefreshTokenLifetime).
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    //the refresh token handle will be updated when refreshing tokens
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    ClientSecrets =
                    {
                        new Secret("internalSecret".Sha256())
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        Scopes.Identity,
                        Scopes.Blog,
                        Scopes.Internal,
                        Scopes.Realtime,
                        Scopes.Messenger,
                        Scopes.FileStorage
                    }
                }
            };
    }
}
