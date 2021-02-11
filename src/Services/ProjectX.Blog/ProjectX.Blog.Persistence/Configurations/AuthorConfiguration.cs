using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectX.Blog.Domain;
using System;

namespace ProjectX.Blog.Persistence.Configurations
{
    internal class AuthorConfiguration : IEntityTypeConfiguration<AuthorEntity>
    {
        public void Configure(EntityTypeBuilder<AuthorEntity> builder)
        {
            builder.ToTable("Author", BlogDbContext.SchemaName);
            builder.Ignore(e => e.DomainEvents);
            builder.HasKey(e => e.Id);

        }
    }
}
