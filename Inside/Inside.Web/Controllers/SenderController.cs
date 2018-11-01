using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inside.Web.Data;
using Inside.Web.Models;
using Inside.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inside.Web.Controllers
{
    [Authorize]
    public class SenderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IPushNotificationService _notificationSender;
        private readonly ApplicationDbContext _context;

        public SenderController(ApplicationDbContext context, IEmailSender emailSender, IPushNotificationService notificationSender, UserManager<ApplicationUser> userManage)
        {
            _emailSender = emailSender;
            _userManager = userManage;
            _notificationSender = notificationSender;
            _context = context;
        }

        public async Task<IActionResult> SendEmailIndex(int id)
        {
            var applicationUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            return View(applicationUser);
        }

        [HttpPost]
        public async Task<IActionResult> SendBroadcastEmail(string subject, string message)
        {
            foreach (ApplicationUser user in _userManager.Users)
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            return View("SendEmailIndex");
        }

        [HttpPost]
        public async Task<IActionResult> SendSingleEmail(string email, string subject, string message)
        {
            await _emailSender.SendEmailAsync(email, subject, message);
            return View("SendEmailIndex");
        }


        public IActionResult SendNotificationIndex()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendNotification(string notification)
        {
            _notificationSender.SendNotificationAsync(notification);
            return View("SendNotificationIndex");
        }
    }
}