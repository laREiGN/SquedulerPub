using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using sqeudulerApp.Models;
using sqeudulerApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static sqeudulerApp.Models.TeamPageModel;

namespace sqeudulerApp.Controllers
{
    public class TeamController : Controller
    {
        string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly DB_Context _context;

        public TeamController(DB_Context context)
        {
            _context = context;
        }

        public IActionResult MainPage()
        {
            return View();
        }

        public IActionResult PersonalPage()
        {
            return View();
        }

        public IActionResult SchedulePage()
        {
            return View();
        }

        // t is de unique code of the team
        public IActionResult TeamInfoPage(string t)
        {
            string teamcode = t;

            using SqlConnection conn = new SqlConnection(strCon);
            {
                //sql query. The result  in order is 0. TeamName, 1. City, 2. Code, 3. Address, 4. ZipCode, 5. Owner / 6. UserID
                //note: I use parameters for security reasons
                string teamquery = "SELECT [Teams].[Teamname], [Teams].[TeamCity], [Teams].[TeamCode], [Teams].[TeamAddress]," +
                    "[Teams].[TeamZipCode], concat([User].[FirstName], ' ' ,[User].[LastName])" +
                    "FROM [dbo].[Teams] " +
                    "JOIN [dbo].[User] ON [Teams].[TeamOwner] = [User].[UserId]" +
                    "WHERE [Teams].[TeamCode]= @TeamCode;";

                string membersquery = "SELECT [User].[FirstName], [User].[LastName], [UserTeam].[Role], [User].[PhoneNr]," +
                    "[User].[Email], [User].[UserId]" +
                    "FROM [dbo].[UserTeam] " +
                    "JOIN [dbo].[User] ON [UserTeam].[UserID] = [User].[UserId]" +
                    "WHERE [UserTeam].[Team]= @TeamCode;";

                string rolequery = "SELECT [UserTeam].[Role] " +
                    "FROM [dbo].[UserTeam] " +
                    "JOIN [dbo]. [User] ON [UserTeam].[UserID] = [User].[UserId]" +
                    "WHERE [UserTeam].[Team]= @TeamCode AND [User]. [Email] = @useremail";

                // Create a new list that will contain the 'team information' aka results of the teamquery
                List<string> teaminfo = new List<string>();

                //create a sql command with the team sql query and the original connection string
                using SqlCommand teamcomm = new SqlCommand(teamquery, conn);
                {
                    //here you can give the parameters
                    teamcomm.Parameters.Add("@TeamCode", System.Data.SqlDbType.VarChar);
                    teamcomm.Parameters["@TeamCode"].Value = teamcode;

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = teamcomm.ExecuteReader();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        // for each column in the current row (there should only be one row) add the column info which is in this case
                        // 0. TeamName, 1. City, 2. Code, 3. Address, 4. ZipCode, 5. Owner in that exact order
                        for (int i = 0; i < sqlResultReader.FieldCount; i++)
                        {
                            teaminfo.Add(sqlResultReader.GetValue(i).ToString());
                        }
                    }

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();


                }

                // Create a new list that will contain the members aka the results of the membersquery
                List<List<string>> teammembers = new List<List<string>>();

                //create a sql command with the new sql query and the original connection string
                using SqlCommand memberscomm = new SqlCommand(membersquery, conn);
                {
                    //here you can give the parameters
                    memberscomm.Parameters.Add("@TeamCode", System.Data.SqlDbType.VarChar);
                    memberscomm.Parameters["@TeamCode"].Value = teamcode;

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = memberscomm.ExecuteReader();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        List<string> member = new List<string>();

                        // for each column in the current row (there should only be one row) add the column info which is in this case
                        // 0. TeamName, 1. City, 2. Code, 3. Address, 4. ZipCode, 5. Owner in that exact order
                        for (int i = 0; i < sqlResultReader.FieldCount; i++)
                        {
                            member.Add(sqlResultReader.GetValue(i).ToString());
                        }

                        teammembers.Add(member);
                    }

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();
                }

                // Create a new list that will contain the members aka the results of the membersquery
                string userrole = null;

                //create a sql command with the new sql query and the original connection string
                using SqlCommand rolecomm = new SqlCommand(rolequery, conn);
                {
                    //here you can give the parameters
                    rolecomm.Parameters.Add("@TeamCode", System.Data.SqlDbType.VarChar);
                    rolecomm.Parameters["@TeamCode"].Value = teamcode;
                    rolecomm.Parameters.Add("@useremail", System.Data.SqlDbType.VarChar);
                    rolecomm.Parameters["@useremail"].Value = HttpContext.Session.GetString("Uid");

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = rolecomm.ExecuteReader();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        userrole = sqlResultReader[0].ToString();
                    }

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();
                }

                // create a team context tuple which contains 1. the team information and 2. the members of the team including their information

                Tuple<List<string>, List<List<string>>, string> teamcontext = new Tuple<List<string>, List<List<string>>, string>(teaminfo, teammembers, userrole);

                // add the list to the viewbag dictionary which we can refer to in our html code
                ViewBag.teamcontext = teamcontext;

                
                ///////////Get messages for user
                //List of requests_site, where all the requests are shown
                List<Requests_Site> Requests = new List<Requests_Site>();

                List<Requests_Site> Requests_all = new List<Requests_Site>();


                //get user ID from login session
                int USRID = (int)HttpContext.Session.GetInt32("ID");

                using (_context)
                {

                    var requests_raw = from row in _context.Requests.Where(
                        row => row.Sender_ID == USRID || row.Co_Receiver_ID == USRID)
                                       select row;

                    var requests_all_raw = from row in _context.Requests select row;

                    foreach (var req_raw in requests_raw)
                    {
                        Requests_Site temp_req = new Requests_Site();
                        temp_req.Mssg_ID = req_raw.Mssg_ID;
                        temp_req.Title = req_raw.Title;
                        temp_req.Text = req_raw.Text;
                        temp_req.Sender_ID = req_raw.Sender_ID;
                        //temp_req.Receiver_ID = req_raw.Receiver_ID;
                        temp_req.Team_Code = req_raw.Team_Code;
                        temp_req.Co_Receiver_ID = req_raw.Co_Receiver_ID;
                        temp_req.Co_Recvr_Approved = req_raw.Co_Recvr_Approved;
                        //temp_req.Receiver_Approved = req_raw.Receiver_Approved;
                        temp_req.Date = req_raw.Date;
                        temp_req.Target_Date = req_raw.Target_Date;
                        Requests.Add(temp_req);
                    }

                    //temp forall
                    foreach (var req_raw in requests_all_raw)
                    {
                        Requests_Site temp_req = new Requests_Site();
                        temp_req.Mssg_ID = req_raw.Mssg_ID;
                        temp_req.Title = req_raw.Title;
                        temp_req.Text = req_raw.Text;
                        temp_req.Sender_ID = req_raw.Sender_ID;
                        //temp_req.Receiver_ID = req_raw.Receiver_ID;
                        temp_req.Team_Code = req_raw.Team_Code;
                        temp_req.Co_Receiver_ID = req_raw.Co_Receiver_ID;
                        temp_req.Co_Recvr_Approved = req_raw.Co_Recvr_Approved;
                        //temp_req.Receiver_Approved = req_raw.Receiver_Approved;
                        temp_req.Date = req_raw.Date;
                        temp_req.Target_Date = req_raw.Target_Date;
                        Requests_all.Add(temp_req);
                    }

                    //Linq query for getting team member names
                    var team_members = from usr in _context.User 
                                       from usrtm in _context.UserTeam
                                       where usrtm.Team == teamcode && usr.UserId == usrtm.UserID
                                       select new { usr.FirstName, usr.LastName, usrtm.Role, usr.UserId, usrtm.Team };


                    //Looping through members and request, to assign names to requests
                    foreach (var memb in team_members)
                    {
                        //assign name for logged in user
                        if (memb.UserId == USRID)
                        {
                            ViewBag.Username = memb.FirstName + " " + memb.LastName;
                        }
                        //loop through requests and assign member names
                        foreach (Requests_Site site in Requests)
                        {
                            //assign name for receiver
                            //if (site.Receiver_ID == memb.UserId)
                            //{
                            //    site.Name_Receiver = memb.FirstName + " " + memb.LastName;
                            //}
                            //assign name for co reciever
                            if (site.Co_Receiver_ID == memb.UserId)
                            {
                                site.Name_Co_Receiver = memb.FirstName + " " + memb.LastName;
                            }
                            //assign name to sender
                            if (site.Sender_ID == memb.UserId)
                            {
                                site.Name_Sender = memb.FirstName + " " + memb.LastName;
                            }
                        }

                        foreach (Requests_Site site in Requests_all)
                        {
                            //assign name for receiver
                            //if (site.Receiver_ID == memb.UserId)
                            //{
                            //    site.Name_Receiver = memb.FirstName + " " + memb.LastName;
                            //}
                            //assign name for co reciever
                            if (site.Co_Receiver_ID == memb.UserId)
                            {
                                site.Name_Co_Receiver = memb.FirstName + " " + memb.LastName;
                            }
                            //assign name to sender
                            if (site.Sender_ID == memb.UserId)
                            {
                                site.Name_Sender = memb.FirstName + " " + memb.LastName;
                            }
                        }
                    }


                    ViewBag.UserID = USRID;
                    ViewBag.AllRequests = Requests_all;
                    ViewBag.Teamcode = teamcode;
                    ViewBag.Requests = Requests;
                }               
            }

            return View();
        }


        // POST: Requests1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Mssg_ID,Title,Text,Sender_ID,Team_Code,Co_Receiver_ID,Co_Recvr_Approved,Date")] Requests_Site_Post requests)
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
            new_req.Co_Recvr_Approved = true;
            //new_req.Receiver_Approved = false;
            new_req.Date = DateTime.Now;
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
            /*
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;
            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            */
            return Redirect(Request.Headers["Referer"].ToString());
        }

        public async Task<IActionResult> Delete_request(int id)
        {
            var requests = from row in _context.Requests.Where(
                        row => row.Mssg_ID == id)
                           select row;           
            _context.Requests.Remove(requests.FirstOrDefault());
            await _context.SaveChangesAsync();
            return Redirect(Request.Headers["Referer"].ToString());
        }

        
        
        /*
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create_Request([Bind("Mssg_ID,Title,Text,Sender_ID,Reciever_ID,Team_Code,Co_Reciever_ID,Co_Recvr_Approved,Reciever_Approved,Date")] Requests requests)
        {

            requests.Co_Recvr_Approved = 0;
            requests.Reciever_Approved = 1;
            requests.Date = DateTime.Now;
            if (ModelState.IsValid)
            {
                _context.Add(requests);
                await _context.SaveChangesAsync();
            }
            //return View(requests);
            return Redirect(Request.Headers["Referer"].ToString());
        }
        */
    }
}