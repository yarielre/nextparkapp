using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NextPark.Domain.Entities;
using NextPark.Services.Services.Interfaces;

namespace NextPark.Services.Services.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        private readonly string _password;
        private readonly string _sender;
        private readonly string _senderName;
        private readonly string _username;
        private readonly string _smtpserver;
        private readonly int _smtpserverport;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger) 
        {
            _logger = logger;
            _sender = configuration["Email:Sender"];
            _senderName = configuration["Email:SenderName"];
            _username = configuration["Email:Username"];
            _smtpserver = configuration["Email:SmtpServer"];
            _password = configuration["Email:Password"];
            if (!int.TryParse(configuration["Email:SmtpServerPort"], out _smtpserverport)) _smtpserverport = 0;
        }
      
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_smtpserver)
            {
                Port = _smtpserverport,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            var credentials = new NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;

            try
            {
                var mail = new MailMessage(_sender.Trim(), email.Trim())
                {
                    IsBodyHtml = true,
                    Subject = subject,
                    Body = message
                };

                client.Send(mail);

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Task.CompletedTask;
            }
        }
    }
}
