using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace sqeudulerApp.Models
{
    public class TeamPageModel
    {
        public Teams Teams { get; set; }
        public Requests Requests { get; set; }
        public Requests_Site Requests_On_Site { get; set; }
        public Requests_Site_Post Requests_On_Post { get; set; }
        public Availability availability { get; set; }
        public IEnumerable<User> Users { get; }
        public IEnumerable<Requests> GetAllRequests_Site { get; }

        //Custom Model for showing user names with requests 
        public class Requests_Site
        {
            //Doesn't need additional classes, as it's only used to show extra info, 
            //and doesn't interact with the Database 
            [Key]
            public int Mssg_ID { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
            [Required]
            public int Sender_ID { get; set; }
            [Required]
            public string Team_Code { get; set; }
            public int Co_Receiver_ID { get; set; }
            public bool Co_Recvr_Approved { get; set; }
            //public bool Receiver_Approved { get; set; }
            [Required]
            [DataType(DataType.Date)]
            public DateTime Date { get; set; }
            public string Target_Date { get; set; }
            public string start_work_hour { get; set; }
            public string end_work_hour { get; set; }
            public string Name_Sender { get; set; }
            public string Name_Receiver { get; set; }
            public string Name_Co_Receiver { get; set; }


        }
        //Model for posting a request
        //Can't use integer values in HTML select
        public class Requests_Site_Post
        {
            //Doesn't need additional classes, as it's only used to show extra info, 
            //and doesn't interact with the Database 
            [Key]
            public int Mssg_ID { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
            [Required]
            public string Sender_ID { get; set; }
            [Required]
            public string Team_Code { get; set; }
            public string Co_Receiver_ID { get; set; }
            public string Target_Date { get; set; }
            public string start_work_hour { get; set; }
            public string end_work_hour { get; set; }

        }
    }
}
