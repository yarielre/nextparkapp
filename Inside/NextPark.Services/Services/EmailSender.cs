using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

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
            SmtpClient client = new SmtpClient(_smtpserver);

            client.Port = _port;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(_sender, _password);
            client.EnableSsl = true;
            client.Credentials = credentials;

            var mail = new MailMessage(_sender, email.Trim())
            {
                Subject = subject,
                Body = message
            };

            return client.SendMailAsync(_sender, email, subject, message);
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
