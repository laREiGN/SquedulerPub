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
        //Tom: key should be UserID and Team right? Or else each user can only have 1 team
        [Key]
        [ConcurrencyCheck]
        public int UserID { get; set; }

        //Tom: dit is de team code
        [ForeignKey("Team")]
        [ConcurrencyCheck]
        public string Team { get; set; }

        [ConcurrencyCheck]
        public string Role { get; set; }
    }
}