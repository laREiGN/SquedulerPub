using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{

    public class Availability
    {

        [Key]
        public int id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        [ForeignKey("team_id")]
        public string team_id { get; set; }

        public DateTime work_date { get; set; }

        [DataType(DataType.Time)]
        public DateTime start_work_hour { get; set; }

        [DataType(DataType.Time)]
        public DateTime end_work_hour { get; set; }


    }
}
