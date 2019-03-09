using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NextPark.Models.Models.PushNotification
{
    [JsonObject]
    public class Push
    {
        [JsonProperty("notification_target")]
        public Target Target { get; set; }

        [JsonProperty("notification_content")]
        public Content Content { get; set; }
    }
    [JsonObject]
    public class Content
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("custom_data")]
        public IDictionary<string, string> Payload { get; set; }
    }
    [JsonObject]
    public class Target
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("devices")]
        public IEnumerable Devices { get; set; }
    }

    public class PushResponse
    {
        public object AndroidResponse { get; set; }
        public object IOSResponse { get; set; }
    }

}
