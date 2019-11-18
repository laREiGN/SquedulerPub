using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    public class Teams
    {
        public string Teamname { get; set; }

        public string TeamCity { get; set; }

        public string Description { get; set; }

        [Key]
        public string TeamCode { get; set; }

        public string TeamAddress { get; set; }

        public string TeamZipCode { get; set; }

        public int TeamOwner { get; set; }
    }
}
