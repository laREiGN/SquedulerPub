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
            //new_req.Receiver_ID = int.Parse(requests.Receiver_ID);
            //TODO: Save teamcode in sesion, and get it from said session
            new_req.Team_Code = requests.Team_Code;
            new_req.Co_Receiver_ID = int.Parse(requests.Co_Receiver_ID);
            new_req.Co_Recvr_Approved = false;
            //new_req.Receiver_Approved = false;
            new_req.Date = DateTime.Now;
            //temp will come with start/end date
            DateTime datevalue;

            //check if string has date time format
            if (DateTime.TryParse(requests.Target_Date, out datevalue) && DateTime.TryParse(requests.start_work_hour, out datevalue) && DateTime.TryParse(requests.end_work_hour, out datevalue))
            {
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

        public async Task<IActionResult> Approve_request(int id)
        {
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            requests.FirstOrDefault().Co_Recvr_Approved = true;
            _context.Requests.Update(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }


        public async Task<IActionResult> Disapprove_request(int id)
        {
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            requests.FirstOrDefault().Co_Recvr_Approved = false;
            _context.Requests.Update(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Accept_request(int id)
        {

            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;

            var users = from row in _context.User.Where(
                row => row.UserId == requests.FirstOrDefault().Sender_ID | row.UserId == requests.FirstOrDefault().Co_Receiver_ID)
                        select row;
            Email em = new Email();

            string request_title = requests.FirstOrDefault().Title;
            string request_decription = requests.FirstOrDefault().Text;
            string request_time = requests.FirstOrDefault().Target_Date + "/ Start: " + requests.FirstOrDefault().start_work_hour + "- End: " + requests.FirstOrDefault().end_work_hour;
            string request_date = requests.FirstOrDefault().Date.ToString();

            string sender = "";
            string co = "";
            int sender_id = requests.FirstOrDefault().Sender_ID;
            int co_id = requests.FirstOrDefault().Co_Receiver_ID;
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

            foreach (var usr in users)
            {

                string body3 = "\n Requester: " + sender;
                if (co != "")
                {
                    body3 = body3 + ".\n Co user: " + co;
                }
                string body1 = "Request: " + request_title + " Accepted";
                string body2 = "\n The request has been accepted. \n Description: " + request_decription + ". \n Shift: " + request_time + ". \n Please check your schedule for more information." + body3 + ". \n Request made on: " + request_date;

                em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", usr.Email, body1, body2);

            }

            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Delete_request(int id)
        {
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;

            var users = from row in _context.User.Where(
                row => row.UserId == requests.FirstOrDefault().Sender_ID | row.UserId == requests.FirstOrDefault().Co_Receiver_ID)
                        select row;
            Email em = new Email();

            string request_title = requests.FirstOrDefault().Title;
            string request_decription = requests.FirstOrDefault().Text;
            string request_time = requests.FirstOrDefault().Target_Date + "/ Start: " + requests.FirstOrDefault().start_work_hour + "- End: " + requests.FirstOrDefault().end_work_hour;
            string request_date = requests.FirstOrDefault().Date.ToString();

            string sender = "";
            string co = "";
            int sender_id = requests.FirstOrDefault().Sender_ID;
            int co_id = requests.FirstOrDefault().Co_Receiver_ID;
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


            foreach (var usr in users)
            {
                string body3 = "\n Requester: " + sender;
                if (co != "")
                {
                    body3 = body3 + ".\n Co user: " + co;
                }

                string body1 = "Request: " + request_title + " Denied";
                string body2 = "\n The request has been Denied. \n Description: " + request_decription + ". \n Shift: " + request_time + ". \n Please check your employer for more information." + body3 + ". \n Request made on: " + request_date;

                em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", usr.Email, body1, body2);

            }

            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Delete_request_member(int id)
        {
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;

            var users = from row in _context.User.Where(
                row => row.UserId == requests.FirstOrDefault().Sender_ID | row.UserId == requests.FirstOrDefault().Co_Receiver_ID)
                        select row;
            Email em = new Email();

            string request_title = requests.FirstOrDefault().Title;
            string request_decription = requests.FirstOrDefault().Text;
            string request_time = requests.FirstOrDefault().Target_Date + "/ Start: " + requests.FirstOrDefault().start_work_hour + "- End: " + requests.FirstOrDefault().end_work_hour;
            string request_date = requests.FirstOrDefault().Date.ToString();

            string sender = "";
            string co = "";
            int sender_id = requests.FirstOrDefault().Sender_ID;
            int co_id = requests.FirstOrDefault().Co_Receiver_ID;
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

            foreach (var usr in users)
            {

                string body1 = "Request: " + request_title + " Deleted";
                string body3 = "\n Requester: " + sender;
                if (co != "")
                {
                    body3 = body3 + ".\n Co user: " + co;
                }
                string body2 = "\n The request has been Deleted by the requester. \n Description: " + request_decription + ". \n Shift: " + request_time + ". \n Please check your employer for more information." + body3 + ". \n Request made on: " + request_date;


                em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", usr.Email, body1, body2);

            }

            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

    }
}
