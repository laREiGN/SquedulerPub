using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    public class UserTeam
    {

        public int UserID { get; set; }
        [Key]
        public string Team { get; set; }

        public string Role { get; set; }
    }
}