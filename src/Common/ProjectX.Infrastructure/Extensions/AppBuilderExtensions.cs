using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace ProjectX.Infrastructure.Extensions
{
    public static class AppBuilderExtensions
    {
        #region Swagger

        public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app, string apiName, string swaggerEndpoint)
            => app.UseSwagger()
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
