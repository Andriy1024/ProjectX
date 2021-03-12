using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Collections.Generic;

namespace ProjectX.Infrastructure.Extensions
{
    public static class AppBuilderExtensions
    {
        #region Swagger

        public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app, string apiName, string swaggerEndpoint)
            => app.UseSwagger(c => 
                  {
                      c.PreSerializeFilters.Add((swagger, httpReq) =>
                      {
                           if (httpReq.Headers.TryGetValue("X-Forwarded-Location", out var location))
                           {
                               // Swashbuckle.AspNetCore 5.6.3 versions
                               // It is for incoming requests from a reverse proxy
                               swagger.Servers = new List<OpenApiServer>
                               {
                                    new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/{location}" }
                               };
                           }
                       });
                  })
                  .UseSwaggerUI(options =>
                  {
                       options.DocExpansion(DocExpansion.None);
                       options.DisplayRequestDuration();
                       options.SwaggerEndpoint(swaggerEndpoint, "v1 Documentation");
                       options.DocumentTitle = $"{apiName} Swagger UI";
                       options.OAuthClientId("swagger");
                       options.OAuthClientSecret("swaggerSecret");
                       options.OAuthRealm("swagger-realm");
                       options.OAuthAppName("Swagger");
                  });

        #endregion
    }
}
