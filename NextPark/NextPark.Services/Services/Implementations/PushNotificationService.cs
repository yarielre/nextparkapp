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
        public const string Url = "https://api.appcenter.ms/v0.1/apps/";
        public const string ApiKeyName = "X-API-Token";
        public const string ApiKey = "";// "093b0709a3398b6f9560434dc638c18f13e37f3c"; //"{Your App Center API Token}";
        public const string Organization = "";// "wisegar"; //"{Your organization name}";
        public const string Android = "";// "WorkInPairs"; //"{Your Android App Name}";
        public const string IOS = "";// "WorkInPairs-iOS"; //"{Your iOS App Name}";
        public const string DeviceTarget = "devices_target";
        public class Apis { public const string Notification = "push/notifications"; }

        public PushNotificationService()
        {

        }

        public PushNotificationService(IConfiguration configuration)
        {
            //TODO: Init readonly properties from config file.

        }

        public async Task NotifyAll(string name, string title, string body)
        {

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(ApiKeyName, ApiKey);

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

            client.BaseAddress = new Uri(Url);

            var json = JsonConvert.SerializeObject(push);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var responseAndroid = await client.PostAsync($"{Organization}/{Android}/{Apis.Notification}", content);
            var responseIOS = await client.PostAsync($"{Organization}/{IOS}/{Apis.Notification}", content);

            var resultJsonAndroid = await responseAndroid.Content.ReadAsStringAsync();
            var resultAndroid = JsonConvert.DeserializeObject(resultJsonAndroid);

            var resultJsonIOS = await responseIOS.Content.ReadAsStringAsync();
            var resultIOS = JsonConvert.DeserializeObject(resultJsonIOS);
        }


        public async Task<PushResponse> Notify(ApplicationUser user, string name, string title, string body, IDictionary<string, string> payload = null)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(ApiKeyName, ApiKey);

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
                    Type = DeviceTarget
                }
            };

            client.BaseAddress = new Uri(Url);

            HttpResponseMessage responseIOS = null;
            HttpResponseMessage responseAndroid = null;

            var pushResponse = new PushResponse();

            //For now send try sending to all platforms
            if (user.Devices == null || user.Devices.Count() == 0) return pushResponse;


            push.Target.Devices = user.Devices.Select(d => d.DeviceIdentifier);

            var json = JsonConvert.SerializeObject(push);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            responseIOS = await client.PostAsync($"{Organization}/{IOS}/{Apis.Notification}", content);
            var resultJsonIOS = await responseIOS.Content.ReadAsStringAsync();
            var resultIOS = JsonConvert.DeserializeObject(resultJsonIOS);
            pushResponse.IOSResponse = resultIOS;

            responseAndroid = await client.PostAsync($"{Organization}/{Android}/{Apis.Notification}", content);
            var resultJsonAndroid = await responseAndroid.Content.ReadAsStringAsync();
            var resultAndroid = JsonConvert.DeserializeObject(resultJsonAndroid);
            pushResponse.AndroidResponse = resultAndroid;

            //if (user.Devices.Any(d => d.Platform == DevicePlatform.Ios))
            //{
            //    //TODO try and handle the catched exeption
            //    push.Target.Devices = user.Devices.FindAll(d => d.Platform == DevicePlatform.Ios).Select(d => d.DeviceIdentifier);

            //    var json = JsonConvert.SerializeObject(push);
            //    var content = new StringContent(json, Encoding.UTF8, "application/json");

            //    responseIOS = await client.PostAsync($"{Organization}/{IOS}/{Apis.Notification}", content);
            //    var resultJsonIOS = await responseIOS.Content.ReadAsStringAsync();
            //    var resultIOS = JsonConvert.DeserializeObject(resultJsonIOS);
            //    pushResponse.IOSResponse = resultIOS;
            //}

            //if (user.Devices.Any(d => d.Platform == DevicePlatform.Android))
            //{
            //    //TODO try and handle the catched exeption
            //    push.Target.Devices = user.Devices.FindAll(d => d.Platform == DevicePlatform.Android).Select(d => d.DeviceIdentifier);

            //    var json = JsonConvert.SerializeObject(push);
            //    var content = new StringContent(json, Encoding.UTF8, "application/json");

            //    responseAndroid = await client.PostAsync($"{Organization}/{Android}/{Apis.Notification}", content);
            //    var resultJsonAndroid = await responseAndroid.Content.ReadAsStringAsync();
            //    var resultAndroid = JsonConvert.DeserializeObject(resultJsonAndroid);
            //    pushResponse.AndroidResponse = resultAndroid;
            //}

            return pushResponse;
        }

    }
}
