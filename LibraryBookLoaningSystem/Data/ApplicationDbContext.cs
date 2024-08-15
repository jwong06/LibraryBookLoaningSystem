using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LibraryBookLoaningSystem.IdentityModels;
using LibraryBookLoaningSystem.Models;

namespace LibraryBookLoaningSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Books> Books { get; set; }
        public DbSet<Transactions> Transactions { get; set; }
    }
}

