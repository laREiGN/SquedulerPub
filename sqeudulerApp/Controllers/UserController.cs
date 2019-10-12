using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using sqeudulerApp.Models;
using sqeudulerApp.Services;
using static sqeudulerApp.Scripts.Extra;
namespace sqeudulerApp.Controllers
{
    public class UserController : Controller
    {
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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(String Email)
        {
            if (Email != null)
            {
                if (Email != null)
                {                  
                    //sql query, where it selects all users where the email = the posted email
                    String sql = "SELECT * FROM [dbo].[User] WHERE [Email] = '" + Email.ToString() + "';";
                    //connection string
                    string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                    //create a sql connection with the connection string
                    SqlConnection conn = new SqlConnection(strCon);
                    //create a sql command with the sql query and the connection string 
                    SqlCommand comm = new SqlCommand(sql, conn);
                    //open the connection
                    conn.Open();
                    //create a sql datareader and execute the sql command 
                    SqlDataReader nwReader = comm.ExecuteReader();
                    //returns true, if the sql reader has any rows(wich means it found a result), return false if it has no rows(no/0 results found)
                    bool Found_account = nwReader.HasRows;
                    //close sql reader
                    nwReader.Close();
                    //close sql connection
                    conn.Close();
                    //generate new random temp password
                    string New_password = Generate_Random_String(8);
                    //if a account was found in the sql connection
                    if (Found_account)
                    {
                        //sql query where we chance the old password to the new temp_password, where a user email is the same as the post email
                        String sql2 = "UPDATE [dbo].[User] SET [Password]='"+ New_password + "' WHERE [Email]='" + Email.ToString() + "';";
                        //create a sql connection with the connection string
                        conn = new SqlConnection(strCon);
                        //create a sql command with the new sql query and the original connection string 
                        comm = new SqlCommand(sql2, conn);
                        //open the connection
                        conn.Open();
                        //úse the original sql datareader and execute the new sql command 
                        nwReader = comm.ExecuteReader();
                        //close sql reader
                        nwReader.Close();
                        //close sql connection
                        conn.Close();
                        //sent email to user with the new temp password
                        Sent_Email("smtp.gmail.com", 587, Email, "squeduler@gmail.com", "Password reset", ("Here is your new temporary Password: " + New_password + "."));
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
        }
    }
}