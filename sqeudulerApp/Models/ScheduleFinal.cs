using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
  
    public class ScheduleFinal
    {

        [Key]
        public int id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [ForeignKey("team_id")]
        public string team_id { get; set; }

        public DateTime work_date { get; set; }

        public decimal start_work_hour { get; set; }

        public decimal end_work_hour { get; set; }

        public bool is_holiday { get; set; }

        public bool is_weekend { get; set; }
    }
}
