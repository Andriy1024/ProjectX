using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ProjectX.Common.BlackList;
using ProjectX.Common.Cache;
using ProjectX.Common.Infrastructure.BlackList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProjectX.Common.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        #region Swagger

        public static IServiceCollection AddSwagger(this IServiceCollection services, string apiName, string identityUrl)
             => services.AddSwaggerGen(options =>
                {
                    var info = new OpenApiInfo
                    {
                        Title = $"{Assembly.GetEntryAssembly().GetName().Name}",
                        Version = "v1",
                        Description = $"Welcome to {apiName} swagger API endpoint."
                    };

                    options.SwaggerDoc("v1", info);

                    // Adding auth using login + password
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows()
                        {
                            Password = new OpenApiOAuthFlow()
                            {
                                TokenUrl = new Uri($"{identityUrl}/connect/token"),
                                AuthorizationUrl = new Uri($"{identityUrl}/connect/authorize"),
                                Scopes = new Dictionary<string, string>
                                {
                                    { apiName, $"{apiName} API scope" }
                                }
                            },

                        }
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Id = "oauth2",
                                    Type = ReferenceType.SecurityScheme
                                }
                            },
                            new List<string>()
                        }
                    });

                    // Adding bearer auth
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Description = "Please enter JWT with Bearer into field",
                        Name = "Authorization",
                        Scheme = "Bearer"
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer",
                                },
                                Scheme = "oauth2",
                                Name = "Bearer",
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });

                    List<string> xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly).ToList();
                    xmlFiles.ForEach(xmlFile => options.IncludeXmlComments(xmlFile));
                });

        #endregion

        public static IServiceCollection AddScopedCache(this IServiceCollection services)
             => services.AddScoped(typeof(IScopedCache<,>), typeof(ScopedCache<,>));

        public static IServiceCollection AddSessionBlackListService(this IServiceCollection services)
            => services.AddSingleton<ISessionBlackList, SessionBlackList>();
    }
}
