using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketSystem.Models.Database;
using TicketSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System;

namespace TicketSystem.Controllers
{
    [Authorize]
    public class MsgController : Controller
    {
        private AppDbContext _ctx;
        private readonly UserManager<AppUser> _userManager;
        public MsgController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _ctx = context;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> MsgStart()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Msg/MsgStart" });
            }
            var userId = user.Id;
            var MsgList = await _ctx.MsgUsers
                .Where(a => a.UserId1 == userId || a.UserId2 == userId)
                .Include(chat => chat.Messages.OrderByDescending(m => m.DateTime).Take(1))
                .Include(a => a.Messages)
                .Include(a => a.User1)
                .Include(a => a.User2)
                .ToListAsync();
            return View(MsgList);
        }

        [Authorize]
        public async Task<IActionResult> MsgNew()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Msg/MsgNew" });
            }
            var currentUserId = currentUser.Id;

            var users = await _ctx.Users
                .Where(u => u.Id != currentUserId)
                .ToListAsync();
            return View(users);
        }

        [HttpPost, ValidateAntiForgeryToken, Authorize]
        public async Task<IActionResult> MsgNew(string UserId2)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Msg/MsgNew" });
            }
            var currentUserId = currentUser.Id;

            var chat = new MsgUser
            {
                ChatId = Guid.NewGuid(),
                UserId1 = currentUserId,
                UserId2 = UserId2
            };

            var existingChat = await _ctx.MsgUsers.FirstOrDefaultAsync(c =>
            (c.UserId1 == chat.UserId1 && c.UserId2 == chat.UserId2) ||
            (c.UserId1 == chat.UserId2 && c.UserId2 == chat.UserId1) );

            if (existingChat != null)
            {
                return RedirectToAction("MsgShow", 
                    new
                    {
                        id = existingChat.ChatId
                    } );
            }
            _ctx.MsgUsers.Add(chat);
            var msg = new Msgmsg
            {
                Chatid = chat.ChatId,
                Text = "Dies ist der Beginn des Chats. Diese Nachricht wurde automatisch vom System gesendet.",
                DateTime = DateTime.Now,
                SenderId = chat.UserId1
            };
            _ctx.MsgMsgs.Add(msg);
            await _ctx.SaveChangesAsync();
            return RedirectToAction("MsgShow", new { id = chat.ChatId } );
        }

        public async Task<IActionResult> MsgShow(Guid? id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Msg/MsgStart" });
            }
            var currentUserId = currentUser.Id;

            var msg = await _ctx.MsgMsgs
                .Where(a => a.Chatid == id)
                .Include(a => a.Sender)
                .Include(a => a.Chat).ThenInclude(a => a.User1)
                .Include(a => a.Chat).ThenInclude(a => a.User2)
                .OrderBy(a => a.DateTime)
                .ToListAsync();
            if (msg == null || msg.Count <= 0)
            {
                TempData["NoChat"] = "Es konnte kein Chat gefunden werden.";
                return RedirectToAction(nameof(MsgStart));
            }
            var check = msg.FirstOrDefault()!.Chat;
            if (check.UserId1 != currentUserId && check.UserId2 != currentUserId)
            {
                TempData["NotYourChat"] = "Sie haben keine Berechtigung, sich diesen Chat anzusehen.";
                return RedirectToAction(nameof(MsgStart));
            }
            return View(msg);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMsgs(Guid chatId)
        {
            var messages = await _ctx.MsgMsgs
                .Where(m => m.Chatid == chatId)
                .Include(m => m.Sender)
                .OrderBy(m => m.DateTime)
                .ToListAsync();

            return PartialView("_ChatMessagesPartial", messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMsg([FromBody] MsgSendModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["NoLogin"] = "Kein aktiver Login in session gefunden!";
                return RedirectToAction("AccessDenied", "Account", new { ReturnUrl = $"/Msg/MsgStart" });
            }

            var msg = new Msgmsg
            {
                Chatid = model.ChatId,
                Text = model.Text,
                DateTime = DateTime.Now,
                SenderId = user.Id
            };

            _ctx.MsgMsgs.Add(msg);
            await _ctx.SaveChangesAsync();

            return Ok();
        }

        public class MsgSendModel
        {
            public Guid ChatId { get; set; }
            public string Text { get; set; }
        }
    }
}
