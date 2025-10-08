using library_system.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace library_system.Infrastructure.Persistence.Mappings
{
    /// <summary>
    /// Mapeamento da entidade Loan para o banco de dados.
    /// </summary>
    public class LoanMapping : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            builder.ToTable("Loans");

            builder.HasKey(l => l.LoanId);

            builder.Property(l => l.LoanId)
                   .IsRequired();

            builder.Property(l => l.BookId)
                   .IsRequired();

            builder.Property(l => l.MemberId)
                   .IsRequired();

            builder.Property(l => l.LoanDate)
                   .IsRequired();

            builder.Property(l => l.DueDate)
                   .IsRequired();

            builder.Property(l => l.ReturnDate)
                   .IsRequired(false);

            builder.Property(l => l.Status)
                   .IsRequired();

            builder.Property(l => l.LateFee)
                   .IsRequired(false)
                   .HasColumnType("decimal(18,2)");

            builder.Property(l => l.Notes)
                   .HasMaxLength(500);
        }
    }
}
