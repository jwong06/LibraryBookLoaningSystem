using LibraryBookLoaningSystem.Data;
using LibraryBookLoaningSystem.IdentityModels;
using LibraryBookLoaningSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryBookLoaningSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Authorization;

namespace LibraryBookLoaningSystem.Controllers
{
    public class ReturnController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<Users> userManager;

        static string userId;
        static string userName;

        public ReturnController(ApplicationDbContext db, UserManager<Users> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }
        [Authorize]
        [HttpGet]
        public IActionResult Index(string userId)
        {
            var user = userManager.FindByIdAsync(userId);
            var allReturnableBooks = db.Books;
            var transactionsToCompare = db.Transactions.Where(t => t.UserId == userId && t.TransactionReturnStatus == false);
            List<Transactions> transactionsInfo = new List<Transactions>();

            List<Books> books = new List<Books>();
            foreach (var book in allReturnableBooks)
            {
                foreach (var trans in transactionsToCompare)
                {
                    if(book.BookId == trans.BookId)
                    {
                        books.Add(book);
                        transactionsInfo.Add(trans);
                    }
                }
            }
            ViewBag.TransInfo = transactionsInfo;

            return View(books);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Details(int bookId, int transId, string userId)
        {
            var user = userManager.FindByIdAsync(userId);
            var transactionsToCompare = db.Transactions.Where(t => t.UserId == userId && t.TransactionReturnStatus == false);

            ViewBag.BookId = bookId;

            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            var transactions = db.Transactions.FirstOrDefault(t => t.TransactionId == transId && t.BookId == bookId);
            if (book == null || transactions == null)
            {
                return RedirectToAction("Index", "Return");
            }
            else
            {
                ReturnViewModel returnViewModel = new ReturnViewModel();
                ViewBag.TransInfo = transactions;

                returnViewModel.Books = book;
                returnViewModel.Transactions = transactions;
                return View(returnViewModel);
            }

        }
        [Authorize]
        [HttpPost]
        //Return a book
        public async Task<IActionResult> Details(int bookId, string userId, int transId)
        {
            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            var user = userManager.FindByIdAsync(userId);
            var transactions = db.Transactions.FirstOrDefault(t => t.TransactionId == transId && t.BookId == bookId);


            if (book == null || user == null)
            {
                return RedirectToAction("Index", "Return");
            }
            else
            {
                if (book.BookCopies + 1 < 0)
                {
                    return new StatusCodeResult(400);
                }
                else
                {
                    Transactions model = transactions;
                    model.BookId = bookId;
                    model.UserId = userId;
                    model.TransactionDate = DateTime.Now;
                    model.TransactionReturnStatus = true;
                    book.BookCopies = book.BookCopies + 1;
                    if (book.BookCopies > 0)
                    {
                        book.Status = true;
                    }

                    db.Transactions.Update(model);
                    db.Books.Update(book);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Return");
                }
            }
        }
    }
}
