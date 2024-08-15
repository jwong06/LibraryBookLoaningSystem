using LibraryBookLoaningSystem.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryBookLoaningSystem.Models
{
    public class Transactions
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public Books Book { get; set; }
        [ForeignKey("BookFK")]
        public int BookId { get; set; }
        [Required]
        public Users User { get; set; }
        [ForeignKey("UserFK")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Loan/Return Date")]
        public DateTime TransactionDate { get; set; }
        [Display(Name = "Return By Date")]
        public DateTime ReturnByDate { get; set; }
        public bool TransactionReturnStatus { get; set; }

    }
}
