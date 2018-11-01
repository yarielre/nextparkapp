using Inside.Web.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace TestingNotifications
{
    class Program
    {
       
        static void Main(string[] args)
        {
            
   

            Console.WriteLine("NOTIFICATION TESTER PROGRAM");
            Console.WriteLine();
            do
            {
                Console.WriteLine("Type the message to send: ");
                var message = Console.ReadLine();

                var pushService = new PushNotificationService();

                try
                {
                    pushService.SendNotificationAsync(message);
                    Console.WriteLine("The message was sent...");
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Exception sending message: {0}", e.Message));
                }

                try
                {
                    var scheduledTime = DateTime.Now;
                    scheduledTime = scheduledTime.AddMinutes(5);
                    pushService.ScheduleNotification(string.Format("This messages has been sent after 5 min. Message: {0}", message), scheduledTime);
                    Console.WriteLine("The sheduled message was been scheduled and will be sent after 5 min...");
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("Exception sending scheduled message: {0}", e.Message));
                }


                Console.WriteLine();
                Console.WriteLine();

            } while (true);
        }

        public static string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }
    }
}
