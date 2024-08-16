using LibraryBookLoaningSystem.Data;
using LibraryBookLoaningSystem.Helpers;
using LibraryBookLoaningSystem.IdentityModels;
using LibraryBookLoaningSystem.Models;
using LibraryBookLoaningSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Net;

namespace LibraryBookLoaningSystem.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly ILogger<Books> _logger;
        //private readonly PdfService _pdfService;

        public BookController(ApplicationDbContext db, ILogger<Books> logger /*PdfService pdfService*/)
        {
            this.db = db;
            _logger = logger;
            //_pdfService = pdfService;
        }
        [Authorize]
        public ViewResult Index()
        {
            return View(db.Books.ToList());
        }
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(Books model)
        {
            if (ModelState.IsValid)
            {
                Books books = new Books
                {
                    BookTitle = model.BookTitle,
                    BookDescription = model.BookTitle,
                    BookAuthor = model.BookAuthor,
                    BookCopies = model.BookCopies,
                    DateAdded = DateTime.Now,
                    Status = true
                };

                db.Books.Add(books);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Book");
            }
            return View(model);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Edit(int bookId)
        {
            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return RedirectToAction("Index", "Book");
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
        [HttpPut]
        public async Task<IActionResult> Edit(Books model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            db.Books.Update(model);
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Book");
        }
        [Authorize]
        [HttpGet]
        public IActionResult Details(int bookId)
        {
            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return RedirectToAction("Index", "Book");
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
        [HttpGet]
        public IActionResult DeletePage(int bookId)
        {
            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return RedirectToAction("Index", "Book");
            }
            else
            {
                Books model = new Books();
                model = book;
                return View(model);
            }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Delete(int bookId)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Index", "Book");
            }
            var book = db.Books.FirstOrDefault(b => b.BookId == bookId);
            if (book == null)
            {
                return RedirectToAction("Index", "Book");
            } else
            {
                Books model = new Books();
                model = book;
                db.Books.Remove(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", "Book");
            }
        }
        //public async Task<ActionResult> DownloadAsPdfAsync()
        //{
        //    var file = await _pdfService.GetBooksAsPdf();
        //    return File(file.FileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.FileName);
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
