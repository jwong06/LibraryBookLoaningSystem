using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace LibraryBookLoaningSystem.IdentityModels
{
    public class Users : IdentityUser
    {
        [Display(Name = "First Name")]
        public string FirstName {  get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }
}
