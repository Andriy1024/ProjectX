using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectX.Identity.Domain;

namespace ProjectX.Identity.Persistence.EntityConfigurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.Ignore(u => u.DomainEvents);
            builder.HasKey(u => u.Id);

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            builder.Property(u => u.FirstName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(u => u.LastName)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.OwnsOne(u => u.Address, a => 
            {
                a.Property(a => a.Country).HasColumnName(nameof(AddressObject.Country));
                a.Property(a => a.City).HasColumnName(nameof(AddressObject.City));
                a.Property(a => a.Address).HasColumnName(nameof(AddressObject.Address));
            });
        }
    }
}
