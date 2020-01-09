using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sqeudulerApp.Models;
using sqeudulerApp.Repository;
using sqeudulerApp.Services;
using static sqeudulerApp.Models.TeamPageModel;

namespace sqeudulerApp.Controllers
{
    public class RequestsController : Controller
    {
        private readonly DB_Context _context;

        public RequestsController(DB_Context context)
        {
            _context = context;
        }

        // GET: Requests1
        public async Task<IActionResult> Index()
        {
            return View(await _context.Requests.ToListAsync());
        }

        // GET: Requests1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requests = await _context.Requests
                .FirstOrDefaultAsync(m => m.Mssg_ID == id);
            if (requests == null)
            {
                return NotFound();
            }

            return View(requests);
        }

        // GET: Requests1/Create
        public IActionResult Create()
        {
            return View();
        }

        

        // POST: Requests1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mssg_ID,Title,Text,Sender_ID,Receiver_ID,Team_Code,Co_Receiver_ID,Co_Recvr_Approved,Receiver_Approved,Date,Target_Date,start_work_hour,end_work_hour")] Requests_Site_Post requests)
        {
            Requests new_req = new Requests();
            new_req.Mssg_ID = requests.Mssg_ID;
            new_req.Title = requests.Title;
            new_req.Text = requests.Text;
            new_req.Sender_ID = int.Parse(requests.Sender_ID);
            new_req.Team_Code = requests.Team_Code;
            new_req.Co_Receiver_ID = int.Parse(requests.Co_Receiver_ID);
            new_req.Co_Recvr_Approved = false;
            new_req.Date = DateTime.Now;
            DateTime datevalue;

            //check if string has date time format
            if (DateTime.TryParse(requests.Target_Date, out datevalue) && DateTime.TryParse(requests.start_work_hour, out datevalue) && DateTime.TryParse(requests.end_work_hour, out datevalue))
            {
                //start is after end
                if(DateTime.Parse(requests.start_work_hour) >= DateTime.Parse(requests.end_work_hour))
                {
                    return Redirect(Request.Headers["Referer"].ToString());
                }
                new_req.Target_Date = requests.Target_Date;
                new_req.start_work_hour = requests.start_work_hour;
                new_req.end_work_hour = requests.end_work_hour;
            }
            else
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }
            
            
            if (ModelState.IsValid)
            {
                _context.Add(new_req);
                await _context.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            //return to previous page/
            return Redirect(Request.Headers["Referer"].ToString());
        }


        /// <summary>
        /// Approves a request, when the co_reciever approves a request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Approve_request(int id)
        {
            //select message from database
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            //chance the co reviever approval to true
            requests.FirstOrDefault().Co_Recvr_Approved = true;
            //update the request in the database
            _context.Requests.Update(requests.FirstOrDefault());
            //wait until the changes are saved to the database
            await _context.SaveChangesAsync();
            //return to the previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }

        /// <summary>
        /// Disapproves a request, when the co_reciever doesn't agree with the request anymore
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Disapprove_request(int id)
        {
            //select message from database
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            //chance the co reviever approval to true
            requests.FirstOrDefault().Co_Recvr_Approved = false;
            //update the request in the database
            _context.Requests.Update(requests.FirstOrDefault());
            //wait until the changes are saved to the database
            await _context.SaveChangesAsync();
            //return to the previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }

        /// <summary>
        /// Accepts a request, if with a co_reciever, can only be accepted with a co_approved message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Accept_request(int id)
        {
            //select message from database
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            //select the sender and co reciever(if there is one)
            var users = from row in _context.User.Where(
                row => row.UserId == requests.FirstOrDefault().Sender_ID | row.UserId == requests.FirstOrDefault().Co_Receiver_ID)
                        select row;
            //create email service
            Email em = new Email();
            //assign values from requests to strings
            string request_title = requests.FirstOrDefault().Title;
            string request_decription = requests.FirstOrDefault().Text;
            string request_time = requests.FirstOrDefault().Target_Date + "/ Start: " + requests.FirstOrDefault().start_work_hour + "- End: " + requests.FirstOrDefault().end_work_hour;
            string request_date = requests.FirstOrDefault().Date.ToString();
            string sender = "";
            string co = "";
            //assign ids to int
            int sender_id = requests.FirstOrDefault().Sender_ID;
            int co_id = requests.FirstOrDefault().Co_Receiver_ID;
            //security check
            //has a co_reciever
            if(requests.FirstOrDefault().Co_Receiver_ID >= 0)
            {
                //if request is not accepted
                if(!requests.FirstOrDefault().Co_Recvr_Approved)
                {
                    //return to the previous page
                    return Redirect(Request.Headers["Referer"].ToString());
                }
            }
            //else continue
            //loop through users and assign names, this is not done within the next loop, as the first user, would only get 1(its own) name and not the 2nd persons name
            foreach (var usr in users)
            {
                if (usr.UserId == sender_id)
                {
                    sender = usr.FirstName + " " + usr.LastName;
                }
                if (usr.UserId == co_id)
                {
                    co = usr.FirstName + " " + usr.LastName;
                }
            }
            //send a email(s) to the user(s)
            foreach (var usr in users)
            {
                //assign requester
                string body3 = "\n Requester: " + sender;
                //check if the name of the co_reciever is not empty/there is no co reciever
                if (co != "")
                {
                    body3 = body3 + ".\n Co user: " + co;
                }
                //make title wich says that the request is accepted
                string body1 = "Request: " + request_title + " Accepted";
                //body of the email, showing all information regarding the request
                string body2 = "\n The request has been accepted. \n Description: " + request_decription + ". \n Shift: " + request_time + ". \n Please check your schedule for more information." + body3 + ". \n Request made on: " + request_date;
                //send the email via our support email
                em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", usr.Email, body1, body2);

            }
            //delete the request from the database
            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            //return to the previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Accept_and_approve_request(int id)
        {
            //select message from database
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            //select the sender and co reciever(if there is one)
            var users = from row in _context.User.Where(
                row => row.UserId == requests.FirstOrDefault().Sender_ID | row.UserId == requests.FirstOrDefault().Co_Receiver_ID)
                        select row;
            //create email service
            Email em = new Email();
            //assign values from requests to strings
            string request_title = requests.FirstOrDefault().Title;
            string request_decription = requests.FirstOrDefault().Text;
            string request_time = requests.FirstOrDefault().Target_Date + "/ Start: " + requests.FirstOrDefault().start_work_hour + "- End: " + requests.FirstOrDefault().end_work_hour;
            string request_date = requests.FirstOrDefault().Date.ToString();
            string sender = "";
            string co = "";
            //assign ids to int
            int sender_id = requests.FirstOrDefault().Sender_ID;
            int co_id = requests.FirstOrDefault().Co_Receiver_ID;
                     
            //else continue
            //loop through users and assign names, this is not done within the next loop, as the first user, would only get 1(its own) name and not the 2nd persons name
            foreach (var usr in users)
            {
                if (usr.UserId == sender_id)
                {
                    sender = usr.FirstName + " " + usr.LastName;
                }
                if (usr.UserId == co_id)
                {
                    co = usr.FirstName + " " + usr.LastName;
                }
            }
            //send a email(s) to the user(s)
            foreach (var usr in users)
            {
                //assign requester
                string body3 = "\n Requester: " + sender;
                //check if the name of the co_reciever is not empty/there is no co reciever
                if (co != "")
                {
                    body3 = body3 + ".\n Co user: " + co;
                }
                //make title wich says that the request is accepted
                string body1 = "Request: " + request_title + " Accepted";
                //body of the email, showing all information regarding the request
                string body2 = "\n The request has been accepted. \n Description: " + request_decription + ". \n Shift: " + request_time + ". \n Please check your schedule for more information." + body3 + ". \n Request made on: " + request_date;
                //send the email via our support email
                em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", usr.Email, body1, body2);

            }
            //delete the request from the database
            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            //return to the previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }


        /// <summary>
        /// When a owner wants to delete a request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete_request(int id)
        {
            //select message from database
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            //select the sender and co reciever(if there is one)
            var users = from row in _context.User.Where(
                row => row.UserId == requests.FirstOrDefault().Sender_ID | row.UserId == requests.FirstOrDefault().Co_Receiver_ID)
                        select row;
            //create email service
            Email em = new Email();

            //assign values from requests to strings
            string request_title = requests.FirstOrDefault().Title;
            string request_decription = requests.FirstOrDefault().Text;
            string request_time = requests.FirstOrDefault().Target_Date + "/ Start: " + requests.FirstOrDefault().start_work_hour + "- End: " + requests.FirstOrDefault().end_work_hour;
            string request_date = requests.FirstOrDefault().Date.ToString();

            string sender = "";
            string co = "";
            //assign ids to int
            int sender_id = requests.FirstOrDefault().Sender_ID;
            int co_id = requests.FirstOrDefault().Co_Receiver_ID;

            //loop through users and assign names, this is not done within the next loop, as the first user, would only get 1(its own) name and not the 2nd persons name           
            foreach (var usr in users)
            {
                if (usr.UserId == sender_id)
                {
                    sender = usr.FirstName + " " + usr.LastName;
                }
                if (usr.UserId == co_id)
                {
                    co = usr.FirstName + " " + usr.LastName;
                }
            }
            //send a email(s) to the user(s)
            foreach (var usr in users)
            {
                //assign requester
                string body3 = "\n Requester: " + sender;
                //check if the name of the co_reciever is not empty/there is no co reciever
                if (co != "")
                {
                    body3 = body3 + ".\n Co user: " + co;
                }
                //make title wich says that the request is Denied
                string body1 = "Request: " + request_title + " Denied";
                //body of the email, showing all information regarding the request
                string body2 = "\n The request has been Denied. \n Description: " + request_decription + ". \n Shift: " + request_time + ". \n Please check your employer for more information." + body3 + ". \n Request made on: " + request_date;
                //send the email via our support email
                em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", usr.Email, body1, body2);
            }
            //delete the request from the database
            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            //return to the previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }
        
        /// <summary>
        /// When the requester wants to delete the request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete_request_member(int id)
        {
            //select message from database
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            //select the sender and co reciever(if there is one)
            var users = from row in _context.User.Where(
                row => row.UserId == requests.FirstOrDefault().Sender_ID | row.UserId == requests.FirstOrDefault().Co_Receiver_ID)
                        select row;
            //create email service
            Email em = new Email();

            //assign values from requests to strings
            string request_title = requests.FirstOrDefault().Title;
            string request_decription = requests.FirstOrDefault().Text;
            string request_time = requests.FirstOrDefault().Target_Date + "/ Start: " + requests.FirstOrDefault().start_work_hour + "- End: " + requests.FirstOrDefault().end_work_hour;
            string request_date = requests.FirstOrDefault().Date.ToString();

            string sender = "";
            string co = "";
            //assign ids to int
            int sender_id = requests.FirstOrDefault().Sender_ID;
            int co_id = requests.FirstOrDefault().Co_Receiver_ID;
            //loop through users and assign names, this is not done within the next loop, as the first user, would only get 1(its own) name and not the 2nd persons name  
            foreach (var usr in users)
            {
                if (usr.UserId == sender_id)
                {
                    sender = usr.FirstName + " " + usr.LastName;
                }
                if (usr.UserId == co_id)
                {
                    co = usr.FirstName + " " + usr.LastName;
                }
            }
            //send a email(s) to the user(s)
            foreach (var usr in users)
            {
                //assign requester
                string body3 = "\n Requester: " + sender;
                //check if the name of the co_reciever is not empty/there is no co reciever
                if (co != "")
                {
                    body3 = body3 + ".\n Co user: " + co;
                }
                //make title wich says that the request is Deleted by the user
                string body1 = "Request: " + request_title + " Deleted";
                //body of the email, showing all information regarding the request
                string body2 = "\n The request has been Deleted by the requester. \n Description: " + request_decription + ". \n Shift: " + request_time + ". \n Please check your employer for more information." + body3 + ". \n Request made on: " + request_date;
                //send the email via our support email
                em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", usr.Email, body1, body2);
            }
            //delete the request from the database
            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            //return to the previous page
            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}
