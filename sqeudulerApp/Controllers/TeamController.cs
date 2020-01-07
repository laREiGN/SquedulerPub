using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sqeudulerApp.Models;
using sqeudulerApp.Services;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using static sqeudulerApp.Scripts.Extra;
using System.Globalization;
using sqeudulerApp.Repository;
using static sqeudulerApp.Models.TeamPageModel;


namespace sqeudulerApp.Controllers
{
    [Route("[controller]")]
    public class TeamController : Controller
    {

        private readonly ICalendar _Calendar;
        private readonly IUser _User;
        private readonly ITeams _Teams;
        private readonly IUserTeam _UserTeam;
        private readonly DB_Context _context;
        private readonly IAvailability _Availability;

        string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public TeamController(IAvailability _IAvailability, ICalendar _ICalendar , IUser _IUser, ITeams _ITeams, IUserTeam _IUserTeam, DB_Context context)
        {
            _Calendar = _ICalendar;
            _User = _IUser;
            _Teams = _ITeams;
            _UserTeam = _IUserTeam;
            _context = context;
            _Availability = _IAvailability;
        }

        [Route("[action]/{Email}/{TeamCode}")]
        public IActionResult RemoveUserFromTeam(string Email, string TeamCode) 
        {
            //call method EmailToID with email to retrieve user id
            int UserID = _User.EmailToID(Email);
            _UserTeam.Remove(UserID, TeamCode);
            return RedirectToAction("TeamInfoPage", "Team", new { t = TeamCode });
        }

        [Route("[action]/{Email}/{TeamCode}")]
        public IActionResult PromoteUserInTeam(string Email, string TeamCode)
        {
            //call method EmailToID with email to retrieve user id
            int UserID = _User.EmailToID(Email);
            _UserTeam.PromoteUserInTeam(UserID, TeamCode);
            return RedirectToAction("TeamInfoPage", "Team", new { t = TeamCode });

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
        [Route("[action]/{t}")]
        public IActionResult TeamInfoPage(string t)
        {
            string teamcode = t;
            
            using SqlConnection conn = new SqlConnection(strCon);
            {
                //sql query. The result  in order is 0. TeamName, 1. City, 2. Code, 3. Address, 4. ZipCode, 5. Owner
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

                string availabilityquery = "SELECT [Availability].[work_date], [Availability].[start_work_hour], " +
                    "[Availability].[end_work_hour]" +
                    "FROM [dbo].[Availability]" +
                    "JOIN [dbo].UserTeam ON [UserTeam].[UserID] = [Availability].[UserId]" +
                    "WHERE [UserTeam].[Team] = @TeamCode AND [UserTeam].[UserId] = [Availability].[UserId]";

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

                // Create a new list that will contain the availability information aka results of the teamquery
                List<List<string>> availability = new List<List<string>>();

                //create a sql command with the team sql query and the original connection string
                using SqlCommand availabilityconn = new SqlCommand(availabilityquery, conn);
                {
                    //here you can give the parameters
                    availabilityconn.Parameters.Add("@TeamCode", System.Data.SqlDbType.VarChar);
                    availabilityconn.Parameters["@TeamCode"].Value = teamcode;

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = availabilityconn.ExecuteReader();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        List<string> singleavailability = new List<string>();
                        // for each column in the current row (there should only be one row) add the column info which is in this case
                        // 0. date, 1. start time, 2. end time
                        for (int i = 0; i < sqlResultReader.FieldCount; i++)
                        {
                            singleavailability.Add(sqlResultReader.GetValue(i).ToString());
                        }
                        DateTime date1 = DateTime.ParseExact(singleavailability[0], "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime time1 = DateTime.ParseExact(singleavailability[1], "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                        DateTime time2 = DateTime.ParseExact(singleavailability[2], "M/d/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

                        List<string> singleavailabilityupdate = new List<string>();
                        singleavailabilityupdate.Add(date1.ToString("dd/MM/yyyy"));
                        singleavailabilityupdate.Add(time1.ToString("HH:mm"));
                        singleavailabilityupdate.Add(time2.ToString("HH:mm"));
                        availability.Add(singleavailabilityupdate);
                    }

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();


                }

                // create a team context tuple which contains 1. the team information and 2. the members of the team including their information
                Tuple<List<string>, List<List<string>>, string, List<List<string>>> teamcontext = new Tuple<List<string>, List<List<string>>, string, List<List<string>>>(teaminfo, teammembers, userrole, availability);

                // add the list to the viewbag dictionary which we can refer to in our html code
                ViewBag.teamcontext = teamcontext;

                //Create list from ScheduleFinal table (get all schedules where teamcode matches) to list()
                List<Calendar> list = _Calendar.GetCalendar.Where(x => x.TeamId == teamcode).ToList();
                //Add list to ViewBag.Calendar
                ViewBag.Calendar = list;
                //Get amount of rows in list and add to ViewBag.CalCount. (i couldnt use count in ViewBag.Calendar in TeamInfoPage, so i came up with this)
                ViewBag.CalCount = list.Count();

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

        [HttpPost]
        public IActionResult ProvideAvailability(UserAvailability model)
        {
            if (ModelState.IsValid)
            {
                // takes current session user id (email in this case)
                string currentUser = HttpContext.Session.GetString("Uid");

                //connection opened to database
                using SqlConnection conn = new SqlConnection(strCon);

                // sql query, that reads userid's associated to the current users email
                string query = "SELECT [UserId] FROM [dbo].[User] WHERE [Email]= '" + currentUser + "';";

                // block of code, that ready the results of the above query
                using SqlCommand comm = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader sqlResultReader = comm.ExecuteReader();

                //checks if the query actually returned any data
                if (sqlResultReader.Read())
                {
                    // the top result of the above query is saved as team owner in the teams table, and the reader is closed
                    int currentUserID = Convert.ToInt32(sqlResultReader[0].ToString());
                    model.availability.UserId = currentUserID;
                    conn.Close();

                    _Availability.Add(model.availability);

                    //opens correct teampage again
                    return RedirectToAction("TeamInfoPage", "Team", new { t = model.availability.team_id });
                }
            }
            return RedirectToAction("TeamInfoPage", "Team", new { t = model.availability.team_id });
        }
    }
}