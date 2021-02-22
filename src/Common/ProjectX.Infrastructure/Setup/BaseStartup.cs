using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Infrastructure.Auth;
using ProjectX.Infrastructure.Extensions;
using ProjectX.Infrastructure.Middleware;
using ProjectX.Core.Setup;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ProjectX.Core;
using ProjectX.Core.JSON;
using ProjectX.Infrastructure.JSON;

namespace ProjectX.Infrastructure.Setup
{
    public abstract class BaseStartup<TOptions>
        where TOptions : BaseOptions
    {
        protected TOptions AppOptions { get; }
        protected string DBConnectionString { get; }
        protected Assembly[] Assemblies { get; }

        protected IWebHostEnvironment Environment { get; }
        protected ILoggerFactory LoggerFactory { get; }
        protected IConfiguration Configuration { get; }

        protected internal BaseStartup(IWebHostEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            Environment = environment;
            LoggerFactory = loggerFactory;
            Configuration = configuration;
            AppOptions = Configuration.Get<TOptions>() ?? throw new ArgumentNullException(nameof(AppOptions));
            DBConnectionString = Configuration.GetConnectionString(nameof(ConnectionStrings.DbConnection)) ?? throw new ArgumentNullException(nameof(ConnectionStrings.DbConnection));
            var paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Application.dll").ToList();
            paths.AddRange(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Infrastructure.dll"));
            Assemblies = paths.Select(path => Assembly.Load(AssemblyName.GetAssemblyName(path))).ToArray();
        }

        public virtual IServiceCollection BaseConfigure(IServiceCollection services)
             => services.AddOptions()
                        .AddHttpContextAccessor()
                        .Configure<BaseOptions>(Configuration)
                        .Configure<ConnectionStrings>(Configuration)
                        .Configure<TOptions>(Configuration)
                        .AddSwagger(AppOptions.ApiName, AppOptions.IdentityUrl)
                        .AddIdentityServerAuthorization()
                        .AddIdentityServerAuthentication(AppOptions.ApiName, AppOptions.IdentityUrl)
                        .AddMediatR(Assemblies)
                        .AddAutoMapper(Assemblies)
                        .AddSingleton<IJsonSerializer, DefaultJsonSerializer>()
                        .AddSingleton<ISystemTextJsonSerializer, SystemTextJsonSerializer>()
                        .AddCors(o => o.AddPolicy("CustomPolicy", 
                                 b => b.AllowAnyOrigin()
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()))
            
                        .AddMvc()
                        .AddJsonOptions(options =>
                        {
                             options.JsonSerializerOptions.Converters.Add(new JsonNonStringKeyDictionaryConverterFactory());
                        })
                        .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = c =>
                        {
                            var errors = string.Join(' ', c.ModelState.Values.Where(v => v.Errors.Count > 0)
                                .SelectMany(v => v.Errors)
                                .Select(v => v.ErrorMessage));

                            return new BadRequestObjectResult(ResponseFactory.InvalidData(ErrorCode.InvalidData, errors));
                        })
                        .AddFluentValidation(t => t.RegisterValidatorsFromAssemblies(Assemblies))
                        .Services;

        public virtual IApplicationBuilder BaseConfigure(IApplicationBuilder app)
              => app.UseRouting()
                    .UseCors("CustomPolicy")
                    .UseMiddleware<ErrorHandlerMiddleware>()
                    .UseAuthentication()
                    .UseAuthorization()
                    .ConfigureSwagger(AppOptions.ApiName, "/swagger/v1/swagger.json")
                    .UseEndpoints(endpoints => {
                        endpoints.MapControllers();
                    });
    }
}
