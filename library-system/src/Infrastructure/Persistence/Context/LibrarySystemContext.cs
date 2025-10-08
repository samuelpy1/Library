using library_system.Domain.Entity;
using library_system.Infrastructure.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace library_system.Infrastructure.Persistence.Context
{
    public class LibrarySystemContext(DbContextOptions<LibrarySystemContext> options) : DbContext(options)
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Loan> Loans { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BookMapping());
            modelBuilder.ApplyConfiguration(new MemberMapping());
            modelBuilder.ApplyConfiguration(new LoanMapping());
        }
    }
}


