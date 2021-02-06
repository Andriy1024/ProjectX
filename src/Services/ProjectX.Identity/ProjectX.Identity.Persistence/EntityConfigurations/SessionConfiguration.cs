using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectX.Identity.Domain;

namespace ProjectX.Identity.Persistence.EntityConfigurations
{
    internal class SessionConfiguration : IEntityTypeConfiguration<SessionEntity>
    {
        public void Configure(EntityTypeBuilder<SessionEntity> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id)
                   .ValueGeneratedNever();

            builder.HasOne(s => s.User)
                   .WithMany(u => u.Sessions)
                   .HasForeignKey(s => s.UserId);

            builder.OwnsOne(s => s.Lifetime, e => 
            {
                e.Property(t => t.AccessTokenExpiresAt).HasColumnName(nameof(SessionLifetime.AccessTokenExpiresAt));
                e.Property(t => t.RefreshTokenExpiresAt).HasColumnName(nameof(SessionLifetime.RefreshTokenExpiresAt));
            });
        }
    }
}
