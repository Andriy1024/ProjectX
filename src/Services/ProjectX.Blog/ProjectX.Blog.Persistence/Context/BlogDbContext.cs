﻿using Microsoft.EntityFrameworkCore;
using ProjectX.Blog.Domain;
using ProjectX.Blog.Persistence.Configurations;
using System.Reflection;

namespace ProjectX.Blog.Persistence
{
    public sealed class BlogDbContext : DbContext
    {
        public const string SchemaName = "ProjectX.Blog";

        public DbSet<AuthorEntity> Authors { get; set; }
        public DbSet<ArticleEntity> Articles { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.ApplyConfiguration(new AuthorConfiguration());
            //builder.ApplyConfiguration(new ArticleConfiguration());
            //builder.ApplyConfiguration(new CommentConfiguration());

            builder.HasDefaultSchema(SchemaName);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            
            base.OnModelCreating(builder);
        }
    }
}
