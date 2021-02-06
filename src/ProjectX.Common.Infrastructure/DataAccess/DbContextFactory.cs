using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectX.Common.Infrastructure.CQRS;
using System;
using System.IO;

namespace ProjectX.Common.Infrastructure.DataAccess
{
    public class DbContextFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
    {
        public T CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("LocalConnection"));

            return Activator.CreateInstance(typeof(T), optionsBuilder.Options) as T;
        }
    }

    public class DbContextWithMediatRFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
    {
        public T CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<T>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("LocalConnection"));

            return Activator.CreateInstance(typeof(T), optionsBuilder.Options, new NoMediator()) as T;
        }
    }
}
