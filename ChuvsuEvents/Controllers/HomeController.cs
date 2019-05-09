using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ChuvsuEvents.Models;
using ChuvsuEvents.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Linq;
namespace ChuvsuEvents.Controllers {
    public class HomeController : Controller {

        private readonly ApplicationContext db;
        private readonly UserManager<User> _userManager;

        public HomeController(UserManager<User> userManager, ApplicationContext context) {
            _userManager = userManager;
            db = context;
        }

        public IActionResult Index() {

            var organizations = from m in db.Organizations select m;
            return View(organizations);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
