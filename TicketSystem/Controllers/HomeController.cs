using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketSystem.Models;
using TicketSystem.Models.Database;

namespace TicketSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly UserManager<AppUser> _userManager;
        public HomeController(AppDbContext context, UserManager<AppUser> userManage)
        {
            _ctx = context;
            _userManager = userManage;
        }

        public async Task<IActionResult> Index()
        {
            var Tickets = await _ctx.Tickets.ToListAsync();
            var Projects = await _ctx.Projects.ToListAsync();
            var UserCount = await _ctx.Users.ToListAsync();
            StatisticViewModel Static = new StatisticViewModel();
            foreach (var ticket in Tickets)
            {
                Static.TicketTotal += 1;
                if (ticket.ReturnStanding() != 4)
                {
                    Static.TicketOpen += 1;
                }
                else
                {
                    Static.TicketClose += 1;
                }
            }
            foreach (var Project in Projects)
            {
                Static.ProjectTotal += 1;
                if (Project.Closed)
                {
                    Static.ProjectClose += 1;
                }
                else
                {
                    Static.ProjectOpen += 1;
                }
            }
            foreach (var user in UserCount)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    Static.Admin += 1;
                }
                else if (await _userManager.IsInRoleAsync(user, "Developer"))
                {
                    Static.Dev += 1;
                }
                else if (await _userManager.IsInRoleAsync(user, "User"))
                {
                    Static.Sup += 1;
                }
            }

            if (User.IsInRole("SuperAdmin") || User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminStart", "Admin");
            }

            return View("Index", Static); //Startseite
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
