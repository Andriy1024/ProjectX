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
    public abstract class BaseStartup
    {
        protected BaseOptions BaseOptions { get; private set; }

        protected IWebHostEnvironment Environment { get; }

        protected ILoggerFactory LoggerFactory { get; }

        protected IConfiguration Configuration { get; }

        protected internal BaseStartup(IWebHostEnvironment environment, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            Environment = environment;
            LoggerFactory = loggerFactory;
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddHttpContextAccessor();
            services.Configure<BaseOptions>(Configuration);
            BaseOptions = Configuration.Get<BaseOptions>();

            services.AddSwagger(BaseOptions.ApiName, BaseOptions.IdentityUrl);

            services.AddIdentityServerAuthorization();
            services.AddIdentityServerAuthentication(BaseOptions.ApiName, BaseOptions.IdentityUrl);

            var paths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Application.dll").ToList();
            paths.AddRange(Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*Infrastructure.dll"));

            Assembly[] assemblies = paths.Select(path => Assembly.Load(AssemblyName.GetAssemblyName(path))).ToArray();

            services.AddMediatR(assemblies);
            services.AddAutoMapper(assemblies);

            services
                .AddMvc()
                .AddJsonOptions(options => { })
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = c =>
                    {
                        var errors = string.Join(' ', c.ModelState.Values.Where(v => v.Errors.Count > 0)
                          .SelectMany(v => v.Errors)
                          .Select(v => v.ErrorMessage));

                        return new BadRequestObjectResult(ResponseFactory.InvalidData(ErrorCode.InvalidData, errors));
                    };
                })
                .AddFluentValidation(t => t.RegisterValidatorsFromAssemblies(assemblies));

            services.AddCors(options =>
            {
                options.AddPolicy("CustomPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod());
            });
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseCors("CustomPolicy");
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.ConfigureSwagger(BaseOptions.ApiName, "/swagger/v1/swagger.json");
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });
        }
    }
}
