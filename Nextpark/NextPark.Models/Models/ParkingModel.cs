using NextPark.Enums.Enums;

namespace NextPark.Models
{
    public class ParkingModel : BaseModel
    {
        public string ImageUrl { get; set; }
        public byte[] ImageBinary { get; set; }

        public string Address { get; set; }
        public int Cap { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string CarPlate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int UserId { get; set; }

        public double PriceMin { get; set; }
        public double PriceMax { get; set; }

        public ParkingStatus Status { get; set; }
    }
}