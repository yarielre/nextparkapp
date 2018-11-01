using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inside.Web.Data;
using Inside.Web.Models;
using Inside.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inside.Web.Controllers
{
    [Authorize]
    public class SendBroadcastNotificationsController : Controller
    {
        PushNotificationService _notificationSender;

        public SendBroadcastNotificationsController()
        {
            _notificationSender = new PushNotificationService();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Send(string notification)
        {
            _notificationSender.SendNotificationAsync(notification);
            return View("Index");
        }
    }
}