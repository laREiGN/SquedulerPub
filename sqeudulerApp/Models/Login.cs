using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    // Not exactly a real user, it is more of the inputs of the pages
    public class Login : User
    {

        [Required]
        [EmailAddress]
        public new string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public new string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

    }
}
