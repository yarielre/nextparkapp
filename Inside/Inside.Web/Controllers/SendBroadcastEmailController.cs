using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inside.Web.Data;
using Inside.Web.Models;
using Inside.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Inside.Web.Controllers
{
    [Authorize]
    public class SendBroadcastEmailController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public SendBroadcastEmailController(ApplicationDbContext context, IEmailSender emailSender, UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string subject, string message)
        {
            foreach (ApplicationUser user in _userManager.Users)
            {
               await _emailSender.SendEmailAsync(user.Email,subject,message);
            }
            return View();
        }
    }
}