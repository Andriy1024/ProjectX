using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core.Auth;
using ProjectX.Infrastructure.Polly;
using System;

namespace ProjectX.Infrastructure.Auth
{
    public static class ServiceCollectionAuthExtensions
    {
        public static IServiceCollection AddIdentityServerAuthorization(this IServiceCollection services)
             => services.AddAuthorization(options =>
                {
                    options.AddPolicy(AuthorizePolicy.Internal, policy => policy.RequireScope("internal"));
                });
        
        public static IServiceCollection AddIdentityServerAuthentication(this IServiceCollection services, string apiName, string usersUrl)
            => services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddIdentityServerAuthentication(options =>
                {
                    options.ApiName = apiName;
                    options.Authority = usersUrl;
                    options.RequireHttpsMetadata = false;
                    options.RoleClaimType = JwtClaimTypes.Role;
                })
                .Services;

        public static void AddTokenProvider(this IServiceCollection services, IConfiguration configuration, string identityUrl, int retryCount = 2)
        {
            services.Configure<TokenProviderOptions>(configuration.GetSection("TokenProviderOptions"));

            services.AddHttpClient("tokenClient", client =>
            {
                client.BaseAddress = new Uri(identityUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddPolicyHandler(RetryPolicies.GetHttpRetryPolicy<TokenProvider>(services, retryCount));

            services.AddSingleton<ITokenProvider, TokenProvider>();
        }
    }
}
