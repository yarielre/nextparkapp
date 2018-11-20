using Newtonsoft.Json;

namespace NextPark.Models
{
    public class TokenResponse
    {
      //  [JsonProperty(PropertyName = "authToken")]
        public string AuthToken { get; set; }

     //   [JsonProperty(PropertyName = "userId")]
        public int UserId { get; set; }

        public bool IsSuccess { get; set; }
    }
}
