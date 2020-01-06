using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    public class UserTeam
    {
        [Key]
        public int UserID { get; set; }
        [ForeignKey("Team")]
        public string Team { get; set; }

        public string Role { get; set; }
    }
}