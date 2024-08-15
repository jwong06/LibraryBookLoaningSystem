using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryBookLoaningSystem.Models
{
    public class Books
    {
        [Key]
        public int BookId { get; set; }
        [Required]
        [Display(Name = "Book Title")]
        public string BookTitle { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Book Description")]
        public string BookDescription { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Book Author")]
        public string BookAuthor { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Copies Available")]
        public int BookCopies { get; set; }
        [Display(Name = "Date Added")]
        public DateTime DateAdded { get; set; }
        [Display(Name = "Active")]
        public bool Status { get; set; }

    }
}
