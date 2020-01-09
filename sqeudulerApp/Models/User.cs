using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    // Not exactly a real user, it is more of the inputs of the pages
    public class User
    {

        [Key]
        public int UserId { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int PhoneNr { get; set; }

        public string Role { get; set; }
    }
}
