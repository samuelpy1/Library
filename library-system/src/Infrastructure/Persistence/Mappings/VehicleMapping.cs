using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using library_system.Domain.Entity;

namespace library_system.Infrastructure.Persistence.Mappings
{
    public class BookMapping : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");

            builder.HasKey(b => b.BookId);

            builder.Property(b => b.BookId)
                   .IsRequired();

            builder.Property(b => b.ISBN)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(b => b.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(b => b.Author)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.Publisher)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.PublicationYear)
                   .IsRequired();

            builder.Property(b => b.Category)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(b => b.TotalCopies)
                   .IsRequired();

            builder.Property(b => b.AvailableCopies)
                   .IsRequired();

            builder.Property(b => b.Status)
                   .IsRequired();
        }
    }
}