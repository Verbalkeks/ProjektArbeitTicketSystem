using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Threading.Tasks;
using TicketSystem.Models;
using TicketSystem.Models.Database;

// VERALTETES CHAT SYSTEM \\
// FÜR SPÄTERE IMPLEMENTIERUNG HIER GELASSEN \\
// JEDOCH NICHT IMPLEMENTIERT! \\

// You can not hide your not-asynced methods from me. -Corin

namespace TicketSystem.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private AppDbContext _ctx;
        private readonly UserManager<AppUser> _userManager;

        public ChatController(AppDbContext ctx, UserManager<AppUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Chat()
        {
            return View(await _ctx.ChatMessages.OrderBy(m => m.Time).ToListAsync());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMsg([FromBody]ChatMessage msg)
        {

            var user = await _userManager.GetUserAsync(User);
            msg.User = user?.UserName ?? "Gast";
            msg.Time = DateTime.Now;

            _ctx.ChatMessages.Add(msg);
            await _ctx.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetMsg()
        {
            var messages = await _ctx.ChatMessages.OrderBy(a => a.Time).ToListAsync();
            return PartialView("_MsgPartial", messages);
        }
    }
}
