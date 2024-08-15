using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryBookLoaningSystem.IdentityModels;
using LibraryBookLoaningSystem.Models;

namespace LibraryBookLoaningSystem.Data
{
    public class BookDbContext : DbContext
    {
        public BookDbContext(DbContextOptions<BookDbContext> options) : base(options)
        {
        }
        public DbSet<Books> Books { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
    }
}
