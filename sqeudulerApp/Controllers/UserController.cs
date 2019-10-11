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
                    String sql = "SELECT * FROM [dbo].[User] WHERE [Email] = '" + Email.ToString() + "';";

                    string strCon = "Server=tcp:squeduler.database.windows.net,1433;Initial Catalog=squeduler;Persist Security Info=False;User ID=user;Password=squeduler#123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

                    SqlConnection conn = new SqlConnection(strCon);
                    SqlCommand comm = new SqlCommand(sql, conn);
                    conn.Open();
                    SqlDataReader nwReader = comm.ExecuteReader();
                    bool Found_account = nwReader.HasRows;
                    nwReader.Close();
                    conn.Close();
                    string New_password = Generate_Random_String(8);
                    if (Found_account)
                    {
                        String sql2 = "UPDATE [dbo].[User] SET [Password]='"+ New_password + "' WHERE [Email]='" + Email.ToString() + "';";
                        conn = new SqlConnection(strCon);
                        comm = new SqlCommand(sql2, conn);
                        conn.Open();
                        nwReader = comm.ExecuteReader();
                        nwReader.Close();
                        conn.Close();                       
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