using Microsoft.Extensions.DependencyInjection;
using ProjectX.Infrastructure.Polly;
using System;

namespace ProjectX.Infrastructure.HTTP
{
    public static class HttpServiceCollectionExtensions
    {
        /// <summary>
        /// Adds http client to system
        /// </summary>
        /// <typeparam name="InterfaceType">Type of interface abstraction</typeparam>
        /// <typeparam name="ImplementationType">Type of interface implementation</typeparam>
        /// <param name="services">Instance of IServiceCollection</param>
        /// <param name="url">Represents base address to service</param>
        public static IServiceCollection AddHttpClient<InterfaceType, ImplementationType>(this IServiceCollection services, string url, int retryCount = 2)
            where ImplementationType : BaseHttpClient, InterfaceType
            where InterfaceType : class
        {
            if (string.IsNullOrEmpty(url)) 
            {
                throw new ArgumentNullException($"{typeof(ImplementationType)} url is empty.");
            }
                
            services.AddHttpClient<InterfaceType, ImplementationType>(httpClient =>
            {
                httpClient.BaseAddress = new Uri(url);
            }).AddPolicyHandler(RetryPolicies.GetHttpRetryPolicy<ImplementationType>(services, retryCount));

            return services;
        }
    }
}
