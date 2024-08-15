using System;
using System.Linq;
using LibraryBookLoaningSystem.Models;

namespace LibraryBookLoaningSystem.Data
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Books.Any())
            {
                return;
            }
            var books = new Books()
            {
                BookId = 1,
                BookTitle = "A great guide on C#",
                BookDescription = "A great guide on C#",
                BookAuthor = "John Doe",
                BookCopies = 20,
                DateAdded = DateTime.Now,
                Status = true
            };
            context.Books.Add(books);
            context.SaveChanges();
        }
    }
}
