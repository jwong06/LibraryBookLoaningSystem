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
    public class LoanController : Controller
    {
        private readonly ApplicationDbContext db;
        //private readonly ApplicationDbContext applicationDb;
        private readonly UserManager<Users> userManager;

        static string userId;
        static string userName;

        public LoanController(ApplicationDbContext db, UserManager<Users> userManager)
        {
            this.db = db;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var allActiveBooks = db.Books.Where(b => b.Status == true);
            return View(allActiveBooks.ToList());
        }
        /*[HttpGet]
        public IActionResult LoanableBooks(string userId)
        {
            List<Books> loanableBooks = new List<Books>();
            if (userId == null)
            {
                return new StatusCodeResult(400);
            }
            var user = userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var BookTransactions = db.Transactions.Where(t => t.UserId == userId && t.TransactionReturnStatus != false).ToList();
            ViewBag.ActiveTransactions = BookTransactions;

            var allActiveBooks = db.Books.Where(b => b.Status == true).ToList();
            var result = BookTransactions.IntersectBy(allActiveBooks.Select(x => x.BookId), x => x.BookId);

            foreach (var book in allActiveBooks)
            {
                if(book.BookId)
                {

                }
            //    bool exists = false;
            //    foreach(var transaction in BookTransactions.Where(t => t.BookId == book.BookId))
            //    {

            //    }
            }

            HashSet<int> activeTransactionsHash = new HashSet<int>(activeTransactions.Select(o => o.BookId));
            HashSet<int> allActiveBooksHash = new HashSet<int>(allActiveBooks.Select(o => o.BookId));
            allActiveBooksHash.IntersectWith(activeTransactionsHash);
                foreach(var book in allActiveBooks)
                {
                    foreach(var bookIdHash in allActiveBooksHash)
                    {
                        if(book.BookId != bookIdHash)
                        {
                            loanableBooks.Add(book);
                        }
                    }
                }
            ViewBag.LoanableBooks = loanableBooks;
            LoanController.userId = userId;
            return View(loanableBooks.ToList());
        }*/
        [Authorize]
        [HttpGet]
        public IActionResult Details(int bookId)
        {
            var BookTransactionReturned = db.Transactions.Where(t => t.UserId == userId && t.BookId == bookId && t.TransactionReturnStatus == true).Select(x => x.BookId);
            var BookTransactionNotReturned = db.Transactions.Where(t => t.UserId == userId && t.BookId == bookId && t.TransactionReturnStatus != true).Select(x => x.BookId);
            bool displayLoanBtn = false;
            bool displayReturnBtn = false;
            if (BookTransactionReturned.Contains(bookId) || !BookTransactionNotReturned.Contains(bookId))
            {
                displayLoanBtn = true;
            }
            else if (BookTransactionNotReturned.Contains(bookId))
            {
                displayReturnBtn = true;
            }
            ViewBag.BookId = bookId;
            ViewBag.DisplayLoanBtn = displayLoanBtn;
            ViewBag.DisplayReturnBtn = displayReturnBtn;

            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return RedirectToAction("LoanableBooks", "Loan");
            }
            else
            {
                Books model = new Books();
                model.BookId = book.BookId;
                model.BookTitle = book.BookTitle;
                model.BookDescription = book.BookDescription;
                model.BookAuthor = book.BookAuthor;
                model.BookCopies = book.BookCopies;
                return View(model);
            }
        }
        [Authorize]
        [HttpPost]
        //Loan a book
        public async Task<IActionResult> Details(int bookId, string userId)
        {
            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            var user = userManager.FindByIdAsync(userId);

            if (book == null || user == null)
            {
                return RedirectToAction("LoanableBooks", "Loan");
            }
            else
            {
                if (book.BookCopies - 1 < 0)
                {
                    return new StatusCodeResult(400);
                }
                else
                {
                    Transactions model = new Transactions();
                    model.BookId = bookId;
                    model.UserId = userId;
                    model.TransactionDate = DateTime.Now;
                    model.ReturnByDate = DateTime.Now.AddMonths(1);
                    model.TransactionReturnStatus = false;
                    book.BookCopies = book.BookCopies - 1;
                    if (book.BookCopies == 0)
                    {
                        book.Status = false;
                    }

                    db.Transactions.Update(model);
                    db.Books.Update(book);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Loan");
                }
            }
        }
        [Authorize]
        [HttpPost]
        //Return a book
        public IActionResult Return(int bookId, string userId)
        {
            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            var user = userManager.FindByIdAsync(userId);
            var trans = db.Transactions.FirstOrDefault(t => t.BookId == book.BookId && t.UserId == userId && t.TransactionReturnStatus != true);

            if (book == null || user == null)
            {
                return new StatusCodeResult(400);
            }

            if (trans == null)
            {
                return new StatusCodeResult(404);
            }
            else
            {
                trans.TransactionReturnStatus = true;
                trans.TransactionDate = DateTime.Now;
                book.BookCopies++;
                if (book.BookCopies > 0)
                {  
                    book.Status = true; 
                }
                db.Transactions.Update(trans);
                db.Books.Update(book);
            }
            return RedirectToAction("Index", "Loan");
        }

    }
}
