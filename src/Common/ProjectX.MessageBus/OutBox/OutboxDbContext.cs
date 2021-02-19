using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectX.MessageBus.Outbox;

namespace ProjectX.MessageBus.OutBox
{
    //public sealed class OutboxDbContext : DbContext
    //{
    //    public DbSet<InboxMessage> InboxMessages { get; set; }

    //    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    //    public OutboxDbContext(DbContextOptions<OutboxDbContext> options) 
    //        : base(options)
    //    {
    //    }

    //    protected override void OnModelCreating(ModelBuilder builder)
    //    {
    //        builder.Entity<InboxMessage>(ConfigureInboxMessage);

    //        builder.Entity<OutboxMessage>(ConfigureOutboxMessage);
    //    }

    //    void ConfigureInboxMessage(EntityTypeBuilder<InboxMessage> builder)
    //    {
    //        builder.HasKey(e => e.Id);

    //        builder.Property(e => e.Id)
    //               .ValueGeneratedNever();

    //        builder.Property(e => e.MessageType)
    //               .HasMaxLength(250)
    //               .IsRequired();

    //        builder.Property(e => e.ProcessedAt)
    //               .IsRequired();
    //    }

    //    void ConfigureOutboxMessage(EntityTypeBuilder<OutboxMessage> builder)
    //    {
    //        builder.Ignore(e => e.Message);

    //        builder.Ignore(e => e.Type);

    //        builder.HasKey(e => e.Id);

    //        builder.Property(e => e.Id)
    //               .ValueGeneratedNever();

    //        builder.Property(e => e.MessageType)
    //               .HasMaxLength(250)
    //               .IsRequired();

    //        builder.Property(e => e.SerializedMessage)
    //               .IsRequired();

    //        builder.Property(e => e.SavedAt)
    //               .IsRequired();
    //    }
    //}
}
