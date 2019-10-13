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

        public IActionResult ForgotPassword(string Email)
        {
            if (string.IsNullOrEmpty(Email))
            {
                return View();
            }
            else
            {
                Email em = new Email();
                for (int i = 1; i < 10000; i++)
                {
                    User user = _User.GetUser(i);
                    if (user.Email == Email)
                    {
                        string body1 = "Your password is ";
                        string password = user.Password;
                        string body2 = " . \nPlease change it right away to prevent further log in problems.";
                        string body = body1 + password + body2;
                        em.NewHeadlessEmail("squedrecovery@gmail.com", "squedteam3!", Email, "Password Recovery", body);
                        return View();
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