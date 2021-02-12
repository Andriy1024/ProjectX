using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using ProjectX.Core.Auth;
using System;
using System.Net.Http;

namespace ProjectX.Infrastructure.Polly
{
    public static class RetryPolicies
    {
        public static IAsyncPolicy<HttpResponseMessage> GetHttpRetryPolicy<THttpClient>(IServiceCollection services, int retryCount)
        {
            return HttpPolicyExtensions
                  // HttpRequestException, 5XX and 408  
                  .HandleTransientHttpError()
                  .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                  // Retry two times after delay  
                  .WaitAndRetryAsync(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                      onRetry: (outcome, timespan, retryAttempt, context) =>
                      {
                          var serviceProvider = services.BuildServiceProvider();

                          if (outcome.Result?.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                              serviceProvider.GetService<ITokenProvider>().Clear();

                          serviceProvider.GetService<ILogger<THttpClient>>()
                              .LogWarning($"Delaying for {timespan.TotalMilliseconds}ms, then making retry {retryAttempt}");
                      });
        }
    }
}
