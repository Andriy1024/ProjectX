using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectX.Blog.Domain;
using ProjectX.Blog.Persistence.Configurations;
using ProjectX.Core.DataAccess;
using ProjectX.Infrastructure.DataAccess;

namespace ProjectX.Blog.Persistence
{
    public sealed class BlogDbContext : BaseDbContext<BlogDbContext>, IUnitOfWork
    {
        public const string SchemaName = "ProjectX.Blog";

        public DbSet<AuthorEntity> Authors { get; set; }
        public DbSet<ArticleEntity> Articles { get; set; }
        public DbSet<CommentEntity> Comments { get; set; }

        public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
        {
        }

        public BlogDbContext(DbContextOptions<BlogDbContext> options, IMediator mediator) : base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AuthorConfiguration());
            builder.ApplyConfiguration(new ArticleConfiguration());
            builder.ApplyConfiguration(new CommentConfiguration());

            base.OnModelCreating(builder);
        }
    }
}
