using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sqeudulerApp.Models
{
    public class Requests
    {
       

        [Key]
        //Is generated automaticly 
        public int Mssg_ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        //Uses the ID of the user making the request
        [Required]
        public int Sender_ID { get; set; }

        //Remove, and send request to all owners in the team(just show all request in a team, for all owners)
        //ID of a owner
        //[Required]
        //public int Receiver_ID { get; set; }
        
        //Uses the teamcode of the team, where the request is made in
        [Required]
        public string Team_Code { get; set; }
        //(Optional) The ID of the person, wich this user wants to trade with.(is -1 if the user doesnt want to trade)
        [Required]
        public int Co_Receiver_ID { get; set; }
        //Wether the tagged person approves of the request conditions
        public bool Co_Recvr_Approved { get; set; }
        //Wether the original person still approves of the request conditions
        
        //Remove 
       // public bool Receiver_Approved { get; set; }
        //NOTE: Both Approved values must be 1/true, in order to accept the request, so that both users agree
        //The date of when the message was made
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }
        //[DataType(DataType.Date)]
        //[DataType(DataType.DateTime)]
        public string Target_Date { get; set; }
        //todo
        //[DataType(DataType.DateTime)]
        public string start_work_hour { get; set; }
        //[DataType(DataType.DateTime)]
        public string end_work_hour { get; set; }
    }
}
