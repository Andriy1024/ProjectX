using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectX.Blog.Domain;

namespace ProjectX.Blog.Persistence.Configurations
{
    internal class CommentConfiguration : IEntityTypeConfiguration<CommentEntity>
    {
        public void Configure(EntityTypeBuilder<CommentEntity> builder)
        {
            builder.ToTable("Comment", BlogDbContext.SchemaName);
            builder.Ignore(e => e.DomainEvents);
            builder.HasKey(e => e.Id);

            builder.HasOne(e => e.Author)
                   .WithMany(e => e.Comments)
                   .HasForeignKey(e => e.AuthorId);

            builder.Property(w => w.RowVersion).IsRowVersion();
            builder.Property(e => e.Text).IsRequired();
        }
    }
}
