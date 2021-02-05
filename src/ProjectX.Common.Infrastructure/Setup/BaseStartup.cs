using AutoMapper;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.Common.Infrastructure.Auth;
using ProjectX.Common.Infrastructure.Extensions;
using ProjectX.Common.Infrastructure.Middleware;
using ProjectX.Common.Setup;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProjectX.Common.Infrastructure.Setup
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
            DBConnectionString = Configuration.GetConnectionString("LocalConnection");
            var paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Application.dll").ToList();
            paths.AddRange(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Infrastructure.dll"));
            Assemblies = paths.Select(path => Assembly.Load(AssemblyName.GetAssemblyName(path))).ToArray();
        }

        public virtual void ConfigureServices(IServiceCollection services)
             => services.AddOptions()
                        .AddHttpContextAccessor()
                        .Configure<BaseOptions>(Configuration)
                        .Configure<TOptions>(Configuration)
                        .AddSwagger(AppOptions.ApiName, AppOptions.IdentityUrl)
                        .AddIdentityServerAuthorization()
                        .AddIdentityServerAuthentication(AppOptions.ApiName, AppOptions.IdentityUrl)
                        .AddMediatR(Assemblies)
                        .AddAutoMapper(Assemblies)
                        .AddCors(o => o.AddPolicy("CustomPolicy", 
                                 b => b.AllowAnyOrigin()
                                       .AllowAnyHeader()
                                       .AllowAnyMethod()))
                        .AddMvc()
                        .AddJsonOptions(options => { })
                        .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = c =>
                        {
                            var errors = string.Join(' ', c.ModelState.Values.Where(v => v.Errors.Count > 0)
                                .SelectMany(v => v.Errors)
                                .Select(v => v.ErrorMessage));

                            return new BadRequestObjectResult(ResponseFactory.InvalidData(ErrorCode.InvalidData, errors));
                        })
                        .AddFluentValidation(t => t.RegisterValidatorsFromAssemblies(Assemblies));

        public virtual void Configure(IApplicationBuilder app)
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
