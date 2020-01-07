using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sqeudulerApp.Models;
using sqeudulerApp.Repository;
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
            new_req.Target_Date = requests.Target_Date;
            new_req.start_work_hour = requests.start_work_hour;
            new_req.end_work_hour = requests.end_work_hour;
            
            
            if (ModelState.IsValid)
            {
                _context.Add(new_req);
                await _context.SaveChangesAsync();
                return Redirect(Request.Headers["Referer"].ToString());
            }
            //return to previous page/
            return Redirect(Request.Headers["Referer"].ToString());
        }

        // GET: Requests1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var requests = await _context.Requests.FindAsync(id);
            if (requests == null)
            {
                return NotFound();
            }
            return View(requests);
        }

        // POST: Requests1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Mssg_ID,Title,Text,Sender_ID,Reciever_ID,Team_Code,Co_Reciever_ID,Co_Recvr_Approved,Reciever_Approved,Date")] Requests requests)
        {
            if (id != requests.Mssg_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(requests);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RequestsExists(requests.Mssg_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(requests);
        }

        // GET: Requests1/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Requests1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var requests = await _context.Requests.FindAsync(id);
            _context.Requests.Remove(requests);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RequestsExists(int id)
        {
            return _context.Requests.Any(e => e.Mssg_ID == id);
        }
    }
}
