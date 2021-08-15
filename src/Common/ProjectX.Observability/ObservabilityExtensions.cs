using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using ProjectX.Core.Auth;
using ProjectX.Observability.Tracer;
using Serilog;
using Serilog.Enrichers.Span;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ProjectX.Observability
{
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddTracer(this IServiceCollection services) 
             => services.AddSingleton<ITracer, Tracer.Tracer>();
        
        public static IServiceCollection AddTracerBehaviour(this IServiceCollection services)
             => services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TracerBehaviour<,>));

        public static IServiceCollection AddObservabilityServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment) 
             => services.AddLogging(configuration)
                        .AddOpenTelemetry(environment, configuration)
                        .AddTracer()
                        .AddTracerBehaviour();

        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration) 
             => services.AddLogging(builder =>
                        {
                            var loggerConfig = new LoggerConfiguration()
                                .ReadFrom.Configuration(configuration)
                                .Enrich.WithSpan();

                            Log.Logger = loggerConfig.CreateLogger();
                        
                            builder.ClearProviders() // Clears default console provider.
                                   .AddSerilog();
                        });

        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration configuration) 
        {
            return services.AddOpenTelemetryTracing((serviceProvider, builder) =>
                   {
                       string serviceVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;

                       var serviceName = $"{environment.ApplicationName}.{environment.EnvironmentName}";

                       builder
                           .SetResourceBuilder(ResourceBuilder
                               .CreateDefault()
                               .AddService(serviceName, serviceVersion: serviceVersion, serviceInstanceId: Environment.MachineName))
                               .AddSource(Tracer.Tracer.Name)
                               .AddAspNetCoreInstrumentation(options =>
                               {
                                   options.RecordException = true;
                                   options.Enrich = EnrichTelemetry();
                               })
                               .AddHttpClientInstrumentation(options =>
                               {
                                   options.Filter = message =>
                                       message != null &&
                                       message.RequestUri != null &&
                                      !message.RequestUri.Host.Contains("visualstudio");
                               })
                               .AddSqlClientInstrumentation(options =>
                               {
                                   options.EnableConnectionLevelAttributes = true;
                                   options.SetDbStatementForStoredProcedure = true;
                                   options.SetDbStatementForText = true;
                                   options.RecordException = true;
                               });

                       builder.AddJaegerExporter()
                              .AddConsoleExporter(options => options.Targets = ConsoleExporterOutputTargets.Console);
                   });

            Action<Activity, string, object> EnrichTelemetry() => (activity, @event, @object) =>
            {
                if (@event == "OnStopActivity")
                {
                    ExtractContextFromResponse(activity, @object);
                }
            };

            void ExtractContextFromResponse(Activity activity, object @object)
            {
                if (activity == null) return;

                var httpContext = (@object as HttpResponse)?.HttpContext;

                if (httpContext?.User == null) return;

                var identityId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimType.IdentityId)?.Value;

                if (!string.IsNullOrWhiteSpace(identityId))
                {
                    activity.SetTag("identity.id", identityId);
                }

                var identityRole = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimType.IdentityRole)?.Value;

                if (!string.IsNullOrWhiteSpace(identityRole))
                {
                    activity.SetTag("identity.role", identityRole);
                }
            }
        }
    }
}
