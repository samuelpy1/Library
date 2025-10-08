using library_system.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace library_system.Infrastructure.Persistence.Mappings
{
    public class MemberMapping : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {
            builder.ToTable("Members");

            builder.HasKey(m => m.MemberId);

            builder.Property(m => m.MemberId)
                   .IsRequired();

            builder.Property(m => m.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder
                .Property(m => m.Email)
                .HasConversion(e => e.Address, e => new Domain.ValueObjects.Email(e))
                .IsRequired()
                .HasMaxLength(100);

            builder
                .Property(m => m.Password)
                .HasConversion(p => p.Value, p => new Domain.ValueObjects.Password(p))
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Phone)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(m => m.RegistrationDate)
                   .IsRequired();

            builder.Property(m => m.IsActive)
                   .IsRequired();
        }
    }
}


