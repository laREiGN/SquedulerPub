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

namespace sqeudulerApp.Controllers
{
    public class UserController : Controller
    {
        string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly IUser _User;
        private readonly ITeams _Teams;
        private readonly IUserTeam _UserTeam;
        private readonly IAvailability _Availability;


        public UserController(IUser _IUser, ITeams _ITeams, IUserTeam _IUserTeam, IAvailability _IAvailability)
        {
            _User = _IUser;
            _Teams = _ITeams;
            _UserTeam = _IUserTeam;
            _Availability = _IAvailability;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return View();
            }
            else
            {
                Email em = new Email();
                // for (int i = 1; i < _User.GetUsers.Count(); i++) better solution? ~ Tom
                foreach (User usr in _User.GetUsers)
                {
                    if (usr != null) {
                        if (usr.Email == Email)
                        {
                            //generate new random temp password
                            string New_password = Generate_Random_String(8);
                            //if a account was found in the sql connection

                            //sql query where we chance the old password to the new temp_password, where a user email is the same as the post email
                            String sql2 = "UPDATE [dbo].[User] SET [Password]='" + New_password + "' WHERE [Email]='" + Email.ToString() + "';";
                            //create a sql connection with the connection string
                            SqlConnection conn = new SqlConnection(strCon);
                            //create a sql command with the new sql query and the original connection string
                            SqlCommand comm = new SqlCommand(sql2, conn);
                            //open the connection
                            conn.Open();
                            //úse the original sql datareader and execute the new sql command
                            SqlDataReader nwReader = comm.ExecuteReader();
                            //close sql reader
                            nwReader.Close();
                            //close sql connection
                            conn.Close();
                            //sent email to user with the new temp password


                            string body1 = "Your password is ";
                            string password = New_password;
                            string body2 = " . \nPlease change it right away to prevent further log in problems.";
                            string body = body1 + password + body2;
                            em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", Email, "Password Recovery", body);
                            return RedirectToAction("Index");
                        }
                    }
                }
                return RedirectToAction("Index");
            }

        }

        public IActionResult TeamPage()
        {
            string userEmail = HttpContext.Session.GetString("Uid");
            using SqlConnection conn = new SqlConnection(strCon);
            {
                //sql query. The result is basically the team name first and the team owner second
                //note: I use parameters for security reasons 
                string query = "SELECT [Teams].[Teamname], concat([owner].[FirstName], ' ' ,[owner].[LastName]), [Teams].[Description], [UserTeam].[Team] " +
                    "FROM [dbo].[UserTeam] " +
                    "JOIN [dbo].[Teams] ON [UserTeam].[Team] = [Teams].[TeamCode] " +
                    "JOIN [dbo].[User] AS owner ON [Teams].[TeamOwner] = [owner].[UserId] " +
                    "JOIN [dbo].[User] AS usermember ON [UserTeam].[UserID] = [usermember].[UserId] " +
                    "WHERE [usermember].[Email]= @userEmail;";

                //create a sql command with the new sql query and the original connection string
                using SqlCommand comm = new SqlCommand(query, conn);
                {
                    //here you can give the parameters
                    comm.Parameters.Add("@userEmail", System.Data.SqlDbType.VarChar);
                    comm.Parameters["@userEmail"].Value = userEmail;

                    //open the connection
                    conn.Open();

                    //use the original sql datareader and execute the new sql command
                    SqlDataReader sqlResultReader = comm.ExecuteReader();

                    // Create a new list that will contain the results of the query
                    List<Tuple<string, string, string, string>> teams_of_current_user = new List<Tuple<string, string, string, string>>();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        // add the first item of the current column to the list
                        Tuple<string, string, string, string> teaminfo = new Tuple<string, string, string, string>(sqlResultReader[0].ToString(), sqlResultReader[1].ToString(), sqlResultReader[2].ToString(), sqlResultReader[3].ToString());
                        teams_of_current_user.Add(teaminfo);
                    }
                    // add the list to the viewbag dictionary which we can refer to in our html code
                    ViewBag.teams_of_current_user = teams_of_current_user;

                    //close sql reader
                    sqlResultReader.Close();
                    //close sql connection
                    conn.Close();
                }
            }
            return View("TeamPage");
        }

        [HttpPost]
        public ActionResult JoinTeam(Teams model)
        {
            string TeamCode = model.TeamCode;
            if (string.IsNullOrEmpty(TeamCode) || string.IsNullOrEmpty(model.TeamCode))
            {
                ModelState.AddModelError("UnexistantCode", "This team doesn't exist");
            }
            else
            {
                string query = "SELECT [TeamCode] FROM [dbo].[Teams] WHERE [TeamCode]= '" + TeamCode + "';";
                using SqlConnection conn = new SqlConnection(strCon);
                using SqlCommand comm = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader sqlResultReader = comm.ExecuteReader();
                if (sqlResultReader.Read())
                {
                    string dbTeamCode = sqlResultReader[0].ToString();
                    conn.Close();
                    if (dbTeamCode != null)
                    {
                        // takes current session user id (email in this case)
                        string currentUser = HttpContext.Session.GetString("Uid");

                        //connection opened to database
                        using SqlConnection conn2 = new SqlConnection(strCon);

                        // sql query, that reads userid's associated to the current users email
                        string query2 = "SELECT [UserId] FROM [dbo].[User] WHERE [Email]= '" + currentUser + "';";

                        // block of code, that ready the results of the above query
                        using SqlCommand comm2 = new SqlCommand(query2, conn2);
                        conn2.Open();
                        SqlDataReader sqlResultReader2 = comm2.ExecuteReader();

                        //checks if query returns data
                        if (sqlResultReader2.Read())
                        {
                            // the top result of the above query is saved as a variable
                            int currentUserID = Convert.ToInt32(sqlResultReader2[0].ToString());
                            conn2.Close();

                            //query that checks teams associated with current user
                            using SqlConnection conn3 = new SqlConnection(strCon);
                            string query3 = "SELECT [Team] FROM [dbo].[UserTeam] WHERE [UserID] = '" + currentUserID + "';";
                            using SqlCommand comm3 = new SqlCommand(query3, conn3);
                            conn3.Open();
                            SqlDataReader sqlResultReader3 = comm3.ExecuteReader();
                            //checks if user is actually in any teams
                            if (sqlResultReader3.Read())
                            {
                                List<string> teams = new List<string>();
                                //all teams of user get put into a list for later use
                                while (sqlResultReader3.Read())
                                {
                                    string userTeam = sqlResultReader3[0].ToString();
                                    teams.Add(userTeam);
                                }
                                //if user is already in the team with the code supplied, nothing happens

                                //TODO - ADD ERROR MESSAGE OR NOTIFICATION
                                if (teams.Contains(TeamCode))
                                {
                                    return RedirectToAction("TeamPage");
                                }
                                //if user is not currently in said team, he gets added
                                else
                                {
                                    // the user is added to the group
                                    UserTeam model2 = new UserTeam();
                                    model2.UserID = currentUserID;
                                    model2.Team = TeamCode;
                                    model2.Role = "member";
                                    _UserTeam.Add(model2);
                                    return RedirectToAction("TeamPage");
                                }
                            }
                            //if user is not in any groups, he gets added to the team with the provided code
                            else
                            {
                                // the user is added to the group
                                UserTeam model2 = new UserTeam();
                                model2.UserID = currentUserID;
                                model2.Team = TeamCode;
                                model2.Role = "member";
                                _UserTeam.Add(model2);
                                return RedirectToAction("TeamPage");
                            }
                        }
                    }
                }
                else
                {
                    // TODO: ADD ERROR MESSAGE OR NOTIFICATION THAT TEAM DOES NOT EXIST
                    System.Diagnostics.Debug.WriteLine("TEAM DOES NOT EXIST");
                    return RedirectToAction("TeamPage");
                }
            }
            return RedirectToAction("TeamPage");
        }

        [HttpPost]
        public IActionResult Create(User model)
        {
            // used for the sign up button. when you click on that button,
            // the button will take the input within that form in the html and create a User object out of it and pass that as argument when it calls this action
            // since the form only asks for email and password when loggin in, the FirstName Field remains null. Tom
            if (model.FirstName is null)
            {
                return View();
            }

            else if (ModelState.IsValid)
            {
                _User.Add(model);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateTeam(Teams model)
        {
            if (ModelState.IsValid)
            {
                // generates a random code, later used to connect to team
                string teamCode = Generate_Random_String(10);
                model.TeamCode = teamCode;

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
                    model.TeamOwner = currentUserID;
                    conn.Close();

                    //the new team is added to the database
                    _Teams.Add(model);

                    //adds current user to the new team (userteam table)
                    UserTeam model2 = new UserTeam();
                    model2.UserID = currentUserID;
                    model2.Team = teamCode;
                    model2.Role = "admin";
                    _UserTeam.Add(model2);

                    //opens teampage again
                    return RedirectToAction("TeamPage");
                }
            }
            return RedirectToAction("TeamPage");
        }


        [HttpPost]
        public IActionResult Login(User model)
        {
            string Email = model.Email;
            string Password = model.Password;
            if (ModelState.IsValid && Email != null && Password != null)
            {
                //create a sql connection with the connection string
                using SqlConnection conn = new SqlConnection(strCon);
                {
                    //sql query where we search for the email address in the database with the associated password. Only 1 result should be found.
                    //note: I use parameters for security reasons
                    string query = "SELECT [Email], [Password] FROM [dbo].[User] WHERE [Email]= @email;";

                    //create a sql command with the new sql query and the original connection string
                    using SqlCommand comm = new SqlCommand(query, conn);
                    {
                        //here you can give the parameters
                        comm.Parameters.Add("@email", System.Data.SqlDbType.VarChar);
                        comm.Parameters["@email"].Value = Email;
                        // comm.Parameters.Add("@password", System.Data.SqlDbType.VarChar);
                        // comm.Parameters["@password"].Value = Password;

                        //open the connection
                        conn.Open();

                        //use the original sql datareader and execute the new sql command
                        SqlDataReader sqlResultReader = comm.ExecuteReader();
                        if (sqlResultReader.Read() && sqlResultReader.VisibleFieldCount == 2)
                        {
                            // There should be 1 row and 2 columns meaning 2 fields (row * columns)
                            if (sqlResultReader[0].ToString() == Email && sqlResultReader[1].ToString() == Password)
                            {
                                //close sql reader
                                sqlResultReader.Close();
                                //close sql connection
                                conn.Close();

                                //safe user id to session so we can use it later to retrieve data from the database, it accepts two strings
                                // one for the key and one for the value
                                // use this to retrieve string HttpContext.Session.GetString("Uid")
                                // it accepts a key and returns the string value
                                // you can also use a viewbag to transport values to html views
                                HttpContext.Session.SetString("Uid", Email);

                                return RedirectToAction("TeamPage", "User");
                            }
                            else
                            {
                                sqlResultReader.Close();
                                conn.Close();
                                return View("Index");
                            }
                        }
                        else { return RedirectToAction("Index", "User"); }
                    }
                }

            }
            else { return RedirectToAction("Index", "User"); }
        }
        public ActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult GoToTeam(string team)
        {
            return RedirectToAction("TeamInfoPage", "Team", new { t = team});
        }

        public IActionResult DeleteTeam(string TeamId)
        {
            //check if session extist
            if (HttpContext.Session.GetString("Uid") == null)
            {
                return Redirect("Index");
            }
            //put session in string var Email
            string Email = HttpContext.Session.GetString("Uid");
            //call method EmailToID with email to retrieve user id
            int UserID = _User.EmailToID(Email);
            //call CheckAdminOrNot method with parameters id and teamcode and check if user is admin or member
            bool check = _UserTeam.CheckAdminOrNot(UserID, TeamId);
            if (check == true)
            {
                _Teams.Remove(TeamId);
                return Redirect("TeamPage");
            }
            else
            {
                _UserTeam.Remove(UserID, TeamId);
            }
            return Redirect("TeamPage");
        }

    }
}
