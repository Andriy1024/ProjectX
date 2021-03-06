﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectX.Core.Setup;
using System;
using System.IO;

namespace ProjectX.DataAccess
{
    public class DbContextFactory<T> : IDesignTimeDbContextFactory<T> where T : DbContext
    {
        public T CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                //.AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(nameof(ConnectionStrings.DbConnection));

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("ConnectionString is empty.");

            var optionsBuilder = new DbContextOptionsBuilder<T>();

            optionsBuilder.UseNpgsql(connectionString);

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
                //.AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString(nameof(ConnectionStrings.DbConnection));

            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("ConnectionString is empty.");

            var optionsBuilder = new DbContextOptionsBuilder<T>();

            optionsBuilder.UseNpgsql(connectionString);

            return Activator.CreateInstance(typeof(T), optionsBuilder.Options, new NoMediator()) as T;
        }
    }
}
