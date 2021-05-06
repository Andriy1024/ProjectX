using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectX.FileStorage.Application;
using ProjectX.FileStorage.Persistence.Database;
using ProjectX.FileStorage.Persistence.Database.Documents;
using ProjectX.FileStorage.Persistence.FileStorage.Setup;
using ProjectX.Infrastructure.Setup;
using System;

namespace ProjectX.FileStorage.API
{
    public class Startup : BaseStartup<FileStorageAppOptions>
    {
        public Startup(IWebHostEnvironment environment, 
                       ILoggerFactory loggerFactory, 
                       IConfiguration configuration) 
                     : base(environment, loggerFactory, configuration)
        {
        }

        public void ConfigureServices(IServiceCollection services)
                 => BaseConfigure(services)
                   .AddFileStorage()
                   .AddMongo(Configuration)
                   .AddMongoRepository<FileDocument, Guid>("Files");

        public void Configure(IApplicationBuilder app) => BaseConfigure(app);
    }
}
