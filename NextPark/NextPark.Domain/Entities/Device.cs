using NextPark.Domain.Core;
using NextPark.Enums.Enums;

namespace NextPark.Domain.Entities
{
    public class Device : BaseEntity
    {
        public string DeviceIdentifier { get; set; }
        public ApplicationUser User { get; set; }
        public int UserId { get; set; }
        public DevicePlatform Platform { get; set; }
    }
}
