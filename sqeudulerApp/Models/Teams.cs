using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    public class Teams
    {
        [Required]
        public string Teamname { get; set; }

        [Required]
        public string TeamCity { get; set; }

        [Required]
        public string Description { get; set; }

        [Key]
        public string TeamCode { get; set; }

        [Required]
        public string TeamAddress { get; set; }

        [Required]
        public string TeamZipCode { get; set; }

        public int TeamOwner { get; set; }
    }
}
