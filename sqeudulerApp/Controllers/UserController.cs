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
using static sqeudulerApp.Scripts.Extra;
namespace sqeudulerApp.Controllers
{
    public class UserController : Controller
    {
        string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        private readonly IUser _User;

        public UserController(IUser _IUser)
        {
            _User = _IUser;
        }

        public IActionResult Index()
        {
            return View(_User.GetUsers);
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
                for (int i = 1; i < 1000; i++)
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
                return View();
            }
        }

        [HttpPost]
        public IActionResult Create(User model)
        {
            if (ModelState.IsValid)
            {
                _User.Add(model);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}