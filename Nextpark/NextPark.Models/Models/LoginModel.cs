using NextPark.Enums.Enums;

namespace NextPark.Models
{
    public class LoginModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public DevicePlatform Platform { get; set; }
    }
}
