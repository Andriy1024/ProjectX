using IdentityModel;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectX.Common.Infrastructure.Auth
{
    public static class ServiceCollectionAuthExtensions
    {
        public static void AddIdentityServerAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizePolicy.Internal, policy => policy.RequireScope("internal"));
            });
        }

        public static void AddIdentityServerAuthentication(this IServiceCollection services, string apiName, string usersUrl)
        {
            services.AddAuthentication(options =>
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
            });
            //.AddCookie("cookieAuth");
        }
    }
}
