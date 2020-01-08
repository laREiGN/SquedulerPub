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

        //Uses the teamcode of the team, where the request is made in
        [Required]
        public string Team_Code { get; set; }
        //(Optional) The ID of the person, wich this user wants to trade with.(is -1 if the user doesnt want to trade)
        [Required]
        public int Co_Receiver_ID { get; set; }
        //Wether the tagged person approves of the request conditions
        public bool Co_Recvr_Approved { get; set; }
        //Wether the original person still approves of the request conditions
      
        //The date of when the message was made
        [DataType(DataType.DateTime)]
        public DateTime Date { get; set; }       
        public string Target_Date { get; set; }       
        public string start_work_hour { get; set; }      
        public string end_work_hour { get; set; }
    }
}
