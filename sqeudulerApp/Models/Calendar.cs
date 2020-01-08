using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    public class Calendar
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        public string TeamId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsHoliday { get; set; }
        public bool IsWeekend { get; set; }
    }
}
