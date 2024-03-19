using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Lista10.Data;
using Lista10.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Lista10.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;

        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Test()
        {
            return View();
        }

        public IActionResult TestHtmlHelper()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "Sysop")]
        public IActionResult Info()
        {
            var numberOfUsersInDatabase = _context.Users.Count();
            ViewBag.Message = "Number of users: " + numberOfUsersInDatabase;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult ForAdmin()
        {ViewData["Info"] = "For Admin"; return View("Info"); }
        [AllowAnonymous]
        public IActionResult ForAll()
        { ViewData["Info"] = "For All"; return View("Info"); }
        [Authorize]
        public IActionResult ForLogIn()
        { ViewData["Info"] = "For Log In"; return View("Info"); }
        [Authorize(Roles = "Admin")]
        public IActionResult ForAdminOrDean()
        { ViewData["Info"] = "For Admin or Dean"; return View("Info"); }

        [Authorize(Policy = "RequireRoleForTurnOnOff")]
        public IActionResult Shutdown()
        {
            return View("Info");
        }
    }
}
