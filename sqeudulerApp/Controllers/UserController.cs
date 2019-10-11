using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using sqeudulerApp.Models;
using sqeudulerApp.Services;

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
    }
}