using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using LibraryBookLoaningSystem.IdentityModels;
using Microsoft.AspNetCore.Identity;
using LibraryBookLoaningSystem.Data;
using LibraryBookLoaningSystem.Models;

namespace LibraryBookLoaningSystem.Helpers
{
    public class PdfService : ITransientDependency
    {
        private readonly IConverter _converter;
        private readonly ApplicationDbContext db;
        public PdfService(IConverter converter, ApplicationDbContext db)
        {
            _converter = converter;
            this.db = db;
        }
        public async Task<FileDto> GetBooksAsPdf()
        {
            var books = db.Books.ToList();
            var html = ConvertUserListToHtmlTable(books);
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = html
                    }
                }
            };
            return new FileDto("BooksList.pdf", _converter.Convert(doc));
        }

        private string ConvertUserListToHtmlTable(List<Books> books)
        {
            var header1 = "<th>Book Title</th>";
            var header2 = "<th>Book Description</th>";
            var header3 = "<th>Book Author</th>";
            var header4 = "<th>Book Copies</th>";
            var headers = $"<tr>{header1}{header2}{header3}{header4}</tr>";
            var rows = new StringBuilder();
            foreach (var book in books)
            {
                var column1 = $"<td>{book.BookTitle}</td>";
                var column2 = $"<td>{book.BookDescription}</td>";
                var column3 = $"<td>{book.BookAuthor}</td>";
                var column4 = $"<td>{book.BookCopies}</td>";
                var row = $"<tr>{column1}{column2}{column3}{column4}</tr>";
                rows.Append(row);
            }
            return $"<table>{headers}{rows.ToString()}</table>";
        }
    }
    public class FileDto
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public FileDto(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }
    }
}
