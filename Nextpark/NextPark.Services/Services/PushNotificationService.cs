using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NextPark.Domain.Entities;
using NextPark.Enums.Enums;
using NextPark.Models.Models.PushNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NextPark.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public readonly string _url;
        public readonly string _apiKeyName;
        public readonly string _apiKey;
        public readonly string _organization;
        public readonly string _android;
        public readonly string _iOS;
        public readonly string _deviceTarget;

        public class Apis
        {
            public const string Notification = "push/notifications";
        }

        public PushNotificationService(IConfiguration configuration)
        {
            _url = configuration["AppCenter:Url"];
            _apiKeyName = configuration["AppCenter:ApiKeyName"];
            _apiKey = configuration["AppCenter:ApiKey"];
            _organization = configuration["AppCenter:Organization"];
            _android = configuration["AppCenter:Push:AppNameAndroid"];
            _iOS = configuration["AppCenter:AppNameiOS"];
            _deviceTarget = configuration["AppCenter:DeviceTarget"];
        }
       

        public async Task NotifyAllAsync(string name, string title, string body)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(_apiKeyName, _apiKey);

            var payload = new Dictionary<string, string>();

            var push = new Push
            {
                Content = new Content
                {
                    Name = name,
                    Title = title,
                    Body = body,
                    Payload = payload
                }
            };

            client.BaseAddress = new Uri(_url);

            var json = JsonConvert.SerializeObject(push);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseAndroid = await client.PostAsync($"{_organization}/{_android}/{Apis.Notification}", content);
            var responseIOS = await client.PostAsync($"{_organization}/{_iOS}/{Apis.Notification}", content);

            var resultJsonAndroid = await responseAndroid.Content.ReadAsStringAsync();
            var resultAndroid = JsonConvert.DeserializeObject(resultJsonAndroid);

            var resultJsonIOS = await responseIOS.Content.ReadAsStringAsync();
            var resultIOS = JsonConvert.DeserializeObject(resultJsonIOS);
        }


        public async Task<PushResponse> NotifyAsync(ApplicationUser user, string name, string title, string body,
            IDictionary<string, string> payload = null)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(_apiKeyName, _apiKey);

            var push = new Push
            {
                Content = new Content
                {
                    Name = name,
                    Title = title,
                    Body = body,
                    Payload = payload ?? new Dictionary<string, string>()
                },
                Target = new Target
                {
                    Type = _deviceTarget
                }
            };

            client.BaseAddress = new Uri(_url);

            HttpResponseMessage responseIOS = null;
            HttpResponseMessage responseAndroid = null;

            var pushResponse = new PushResponse();

            if (user.Devices.Any(d => d.Platform == DevicePlatform.Ios))
            {
                //TODO try and handle the catched exeption
                push.Target.Devices = user.Devices.FindAll(d => d.Platform == DevicePlatform.Ios)
                    .Select(d => d.DeviceIdentifier);

                var json = JsonConvert.SerializeObject(push);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                responseIOS = await client.PostAsync($"{_organization}/{_iOS}/{Apis.Notification}", content);
                var resultJsonIOS = await responseIOS.Content.ReadAsStringAsync();
                var resultIOS = JsonConvert.DeserializeObject(resultJsonIOS);
                pushResponse.IOSResponse = resultIOS;
            }

            if (user.Devices.Any(d => d.Platform == DevicePlatform.Android))
            {
                //TODO try and handle the catched exeption
                push.Target.Devices = user.Devices.FindAll(d => d.Platform == DevicePlatform.Android)
                    .Select(d => d.DeviceIdentifier);

                var json = JsonConvert.SerializeObject(push);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                responseAndroid = await client.PostAsync($"{_organization}/{_android}/{Apis.Notification}", content);
                var resultJsonAndroid = await responseAndroid.Content.ReadAsStringAsync();
                var resultAndroid = JsonConvert.DeserializeObject(resultJsonAndroid);
                pushResponse.AndroidResponse = resultAndroid;
            }

            return pushResponse;
        }

        //    #region Attributes
        //    private readonly string ListenConnectionString = "Endpoint=sb://insideparking.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=/IpzF90pthfg5B9X+hTPuflyIHHzBcp9yltNAUWXP/g=";
        //    private readonly string NotificationHubName = "InsideParkingHub";
        //    private NotificationHubClient hub;
        //    private readonly Timer timer;
        //    #endregion

        //    #region Properties
        //    public string NotificationStorage { get; }
        //    public object Locker { get; } = new object();
        //    #endregion

        //    public PushNotificationService()
        //    {
        //        hub = NotificationHubClient.CreateClientFromConnectionString(ListenConnectionString, NotificationHubName);
        //        NotificationStorage = Directory.GetCurrentDirectory();//GetApplicationRoot();
        //        timer = new Timer(ProcessScheduledNotification, null, new TimeSpan(0, 0, 0), new TimeSpan(0, 1, 0));
        //    }
        //    private void ProcessScheduledNotification(object state)
        //    {
        //        lock (Locker)
        //        { //Find a better way

        //            var files = Directory.GetFiles(NotificationStorage, "*.notify");

        //            foreach (var file in files)
        //            {
        //                ScheduledSerializable scheduledMessageDeserialized = null;

        //                using (Stream stream = File.OpenRead(file))
        //                {
        //                    try
        //                    {
        //                        var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
        //                        scheduledMessageDeserialized = (ScheduledSerializable)bformatter.Deserialize(stream);
        //                    }
        //                    catch
        //                    {
        //                        continue;
        //                    }

        //                }

        //                if (scheduledMessageDeserialized == null) return;

        //                var difference = (scheduledMessageDeserialized.Scheduled - DateTime.Now).TotalMinutes;

        //                if (Math.Abs(difference) >= 1) return;

        //                SendNotificationAsync(scheduledMessageDeserialized.Message, scheduledMessageDeserialized.Tags);

        //                try
        //                {
        //                    File.Delete(file);
        //                }
        //                catch { }
        //            }

        //        }
        //    }
        //    public void ScheduleNotification(string message, DateTime dateTime, List<string> tags = null)
        //    {
        //        var filename = Guid.NewGuid() + ".notify";

        //        var scheduledMessage = new ScheduledSerializable
        //        {
        //            Message = message,
        //            Scheduled = dateTime,
        //            Tags = tags
        //        };

        //        string serializationFile = Path.Combine(NotificationStorage, filename);

        //        using (Stream stream = File.Open(serializationFile, FileMode.Create))
        //        {
        //            var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

        //            bformatter.Serialize(stream, scheduledMessage);
        //        }
        //    }
        //    public async void SendNotificationAsync(string message, List<string> tags = null)
        //    {
        //        NotificationOutcome gcmResult;
        //        NotificationOutcome apsResult;

        //        if (tags != null && tags.Count > 0)
        //        {
        //            gcmResult = await hub.SendGcmNativeNotificationAsync("{ \"data\" : {\"message\":\"" + message + "\"}}", tags);
        //            apsResult = await hub.SendAppleNativeNotificationAsync("{ \"aps\" : {\"alert\":\"" + message + "\"}}", tags);
        //            return;
        //        }

        //        apsResult = await hub.SendAppleNativeNotificationAsync("{ \"aps\" : {\"alert\":\"" + message + "\"}}");
        //        gcmResult = await hub.SendGcmNativeNotificationAsync("{ \"data\" : {\"message\":\"" + message + "\"}}");
        //    }
        //    /// <summary>
        //    /// This method can be used with the Azure Tair Standard or bigger.
        //    /// </summary>
        //    public async void SendScheduledNotificationAsync(string message, DateTime dateTime, List<string> tags = null)
        //    {
        //        ScheduledNotificationResultSet result = new ScheduledNotificationResultSet();

        //        Notification apsNotification = new AppleNotification("{ \"aps\" : {\"alert\":\"" + message + "\"}}");
        //        Notification gcmNotification = new GcmNotification("{ \"data\" : {\"message\":\"" + message + "\"}}");

        //        if (tags != null && tags.Count > 0)
        //        {
        //            try
        //            {
        //                result.apsResult = await hub.ScheduleNotificationAsync(apsNotification, dateTime, tags);
        //                result.gcmResult = await hub.ScheduleNotificationAsync(gcmNotification, dateTime, tags);
        //            }
        //            catch (Exception e)
        //            {
        //                throw e;
        //            }

        //            return;

        //        }

        //        try
        //        {
        //            result.apsResult = await hub.ScheduleNotificationAsync(apsNotification, dateTime);
        //            result.gcmResult = await hub.ScheduleNotificationAsync(gcmNotification, dateTime);
        //        }
        //        catch (Exception e)
        //        {
        //            throw e;
        //        }

        //        return;

        //    }
        //    public static string GetApplicationRoot()
        //    {
        //        var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
        //        Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
        //        var appRoot = appPathMatcher.Match(exePath).Value;
        //        return appRoot;
        //    }

        //}
        //[Serializable]
        //public class ScheduledSerializable
        //{
        //    public string Message { get; set; }
        //    public DateTime Scheduled { get; set; }
        //    public List<string> Tags { get; set; }
        //}
        //public class NotificationResultSet
        //{
        //    public NotificationOutcome apsResult { get; set; }
        //    public NotificationOutcome gcmResult { get; set; }
        //}
        //public class ScheduledNotificationResultSet
        //{
        //    public ScheduledNotification apsResult { get; set; }
        //    public ScheduledNotification gcmResult { get; set; }
        //}
    }
}
