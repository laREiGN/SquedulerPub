using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    }
}