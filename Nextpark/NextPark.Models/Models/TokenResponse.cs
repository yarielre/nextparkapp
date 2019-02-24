using Newtonsoft.Json;

namespace NextPark.Models
{
    public class TokenResponse
    {

        public string AuthToken { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public bool IsSuccess { get; set; }
    }
}
