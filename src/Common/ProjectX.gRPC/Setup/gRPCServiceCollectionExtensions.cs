using Microsoft.Extensions.DependencyInjection;
using ProjectX.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectX.gRPC.Setup
{
    public static class gRPCServiceCollectionExtensions
    {
        public static IServiceCollection AddgRPC(this IServiceCollection services) 
        {
            services.AddGrpc(o =>
            {
                o.Interceptors.Add<ErrorHandlingInterceptor>();
            });

            return services;
        }

        public static IServiceCollection AddgRPCClient(this IServiceCollection services, string url)
        {
            Utill.ThrowIfNullOrEmpty(url, nameof(url));

            //services.AddGrpcClient<IdentityGRPC.IdentityGRPCClient>(o =>
            //{
            //    o.Address = new Uri(identityUrl);
            //});

            //services.AddScoped<IdentityGrpcClient>();

            return services;
        }
    }
}
