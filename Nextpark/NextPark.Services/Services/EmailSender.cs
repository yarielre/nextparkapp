using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NextPark.Domain.Entities;

namespace NextPark.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class EmailSender : IEmailSender
    {
        string _sender = "";
        string _password = "";
        string _smtpserver = "";
        int _port = 587;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender() //TODO: Inject Configs to get the init settings from json file.
        {
            _sender = "info@insideparking.ch";
            _password = "#$qLPq4gRy";
            _smtpserver = "SSL0.OVH.NET";
            _port = 587;
        }

        public EmailSender(string sender, string password, string smtpserver)
        {
            _sender = sender.Trim();
            _password = password;
            _smtpserver = smtpserver;
        }


        public void SendDebugMessage(string Controller, string Method, string Message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Controller: ");
            sb.Append(Controller);
            sb.AppendLine("Method: ");
            sb.Append(Method);
            sb.AppendLine();
            sb.Append(Message);

            SendEmailAsync("yariel.re@wisegar.ch", "Debug Message", sb.ToString());
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_smtpserver)
            {
                Port = _port,
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

        public void SendEmailToRegisteredUserAsync(ApplicationUser user)
        {
            //TODO Cambiar el asunto del correo 
            var subject = "Docente confirmado";
            var message = new StringBuilder();
            message.Append(
                $"<p>{user.Name} {user.Lastname}, ora fai parte di una community che collega docenti selezionati" +
                " con studenti di tutta la Svizzera. Trova un allievo con cui esplorare" +
                " numerose materie, trasmettere le tue conoscenze e costruire il futuro dell’istruzione.</p>" +
                "<br/>" +
                "<div>Studenti proveniente da tutte le scuole:</div>" +
                "<div> •Elementari</div>" +
                "<div> •Medie</div>" +
                "<div> •Scuole medie superiori</div>" +
                "<div> •Università</div>" +

                "<p>Non ti resta che attendere le tue lezioni, ti notificheremo ogniqualvolta " +
                "troveremo una lezione compatibile con la tua registrazione!</p>" +
               "<p style=\"font-size: 18px\">Inviato con il ♥ da Work IN Pair WORK IN PAIRS " +
                "Sagl, 6500 Bellinzona, +41 79 383 62 33</p>"

            );

            var body = $"<HTML><head></head><body style=\"font-family: Arial;font-size: 12px\">{message.ToString()}</body></HTML>";

            SendEmailAsync(user.Email, subject, body);
        }


        public void SendDemoEmail(string recipient, string username, string password)
        {

            SmtpClient client = new SmtpClient(_smtpserver);
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;

            var sb = new StringBuilder();
            sb.AppendLine("<h1>Ti diamo il benvenuto nella community Work In Pairs</h1>");
            sb.AppendLine("<p>Ciao " + recipient + ", queste sono le tue credenziali per il primo accesso alla piattaforma!</p>");
            sb.AppendLine("<p>Username: " + username + "</p>");
            sb.AppendLine("<p>Password: " + password + "</p>");
            sb.AppendLine("<p>Vai alla piattaforma cliccando  <a href=" + "http://www.workinpairs.ch" + ">qui</a></p>");
            sb.AppendLine("<h4>Vi consigliamo di modificare la password al primo accesso!</h4>");
            sb.AppendLine("<p>Bisogno di aiuto? Contatta l’<a href=" + "mailto:info@workinpairs.ch" + ">equièpe</a> di Work In Pairs</p>");
            sb.AppendLine();
            sb.AppendLine("<h3>Work In Pairs, il tuo partner per la condivisione delle conoscenze</h3>");

            try
            {
                var mail = new MailMessage(_sender.Trim(), recipient.Trim())
                {
                    IsBodyHtml = true,
                    Subject = "Prime credenziali Workinpairs",
                    Body = sb.ToString()
                };
                client.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
