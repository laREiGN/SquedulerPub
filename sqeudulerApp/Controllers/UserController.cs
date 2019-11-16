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

        public UserController(IUser _IUser, ITeams _ITeams, IUserTeam _IUserTeam)
        {
            _User = _IUser;
            _Teams = _ITeams;
            _UserTeam = _IUserTeam;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        public IActionResult JoinTeamWindow()
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
                for (int i = 0; i < _User.GetUsers.Count() - 1; i++)
                {
                    User user = _User.GetUser(i);
                    if (user != null){
                        if (user.Email == Email)
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
                //sql query. The result is basically the team name first and the teamcode second
                //note: I use parameters for security reasons
                string query = "SELECT [Teams].[Teamname], [UserTeam].[Team] FROM [dbo].[UserTeam] " +
                    "JOIN [dbo].[User] ON [UserTeam].[UserID] = [User].[UserId]" +
                    "JOIN [dbo].[Teams] ON [UserTeam].[Team] = [Teams].[TeamCode]" +
                    " WHERE [User].[Email]= @userEmail;";

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
                    List<Tuple<string, string>> teams_of_current_user = new List<Tuple<string, string>>();

                    // Iterate through the results of the query a row per itteration
                    while (sqlResultReader.Read())
                    {
                        // add the first item of the current column to the list
                        Tuple<string, string> teaminfo = new Tuple<string, string>(sqlResultReader[0].ToString(), sqlResultReader[1].ToString());
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
        public IActionResult TeamPage(Teams model)
        {
            if (ModelState.IsValid)
            {
                // generates a random code, later used to connect to team
                //TODO - MAKE SURE CODE IS UNIQUE
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
                sqlResultReader.Read();

                // the top result of the above query is saved as team owner in the teams table, and the reader is closed
                int currentUserID = Convert.ToInt32(sqlResultReader[0].ToString());
                model.TeamOwner = currentUserID;
                conn.Close();

                //the new team is added to the database
                _Teams.Add(model);

                //adds current user to the correct team (userteam table)
                UserTeam model2 = new UserTeam();
                model2.UserID = currentUserID;
                model2.Team = teamCode;
                model2.Role = "admin";
                _UserTeam.Add(model2);

                //opens teampage again
                return RedirectToAction("TeamPage");
            }
            return View();
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
        public IActionResult SignOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult GoToTeam(string team)
        {
            return RedirectToAction("TeamInfoPage", "Team", new { t = team});
        }

    }
}
