using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectX.Blog.Domain;

namespace ProjectX.Blog.Persistence.Configurations
{
    internal class ArticleConfiguration : IEntityTypeConfiguration<ArticleEntity>
    {
        public void Configure(EntityTypeBuilder<ArticleEntity> builder)
        {
            builder.ToTable("Article", BlogDbContext.SchemaName);
            builder.Ignore(e => e.DomainEvents);
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Tittle).IsRequired();
            builder.Property(e => e.Body).IsRequired();
            builder.HasMany(e => e.Comments)
                   .WithOne(e => e.Article)
                   .HasForeignKey(e => e.ArticleId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(w => w.RowVersion).IsRowVersion();

            var timeRowsFieldMetadata = builder.Metadata.FindNavigation(nameof(ArticleEntity.Comments));
            timeRowsFieldMetadata.SetField("_comments");
            timeRowsFieldMetadata.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
