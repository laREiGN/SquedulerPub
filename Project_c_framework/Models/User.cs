using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace Project_c_framework.Models
{
    public class User
    {
        public int id { get; set; }
        public String name { get; set; }
        public string password { get; set; }
    }
}
