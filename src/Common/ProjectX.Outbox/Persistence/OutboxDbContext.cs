using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProjectX.Outbox
{
    public sealed class OutboxDbContext : DbContext
    {
        public const string SchemaName = "ProjectX.Outbox";

        public DbSet<InboxMessage> InboxMessages { get; set; }

        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public OutboxDbContext(DbContextOptions<OutboxDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema(SchemaName);

            builder.Entity<InboxMessage>(ConfigureInboxMessage);

            builder.Entity<OutboxMessage>(ConfigureOutboxMessage);
        }

        void ConfigureInboxMessage(EntityTypeBuilder<InboxMessage> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedNever();

            builder.Property(e => e.MessageType)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(e => e.ProcessedAt)
                   .IsRequired();
        }

        void ConfigureOutboxMessage(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.Ignore(e => e.Message);

            builder.Ignore(e => e.Type);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                   .ValueGeneratedNever();

            builder.Property(e => e.MessageType)
                   .HasMaxLength(250)
                   .IsRequired();

            builder.Property(e => e.SerializedMessage)
                   .HasColumnType("jsonb")
                   .IsRequired();

            builder.Property(e => e.SavedAt)
                   .IsRequired();
        }
    }
}
