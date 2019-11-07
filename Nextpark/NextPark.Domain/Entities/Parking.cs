using NextPark.Domain.Core;
using NextPark.Enums.Enums;
using System.Collections.Generic;

namespace NextPark.Domain.Entities
{
    public class Parking : BaseEntity
    {
        public string ImageUrl { get; set; }

        public string Address { get; set; }
        public int Cap { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Notes { get; set; }

        public string CarPlate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double PriceMin { get; set; }
        public double PriceMax { get; set; }

        public ParkingStatus Status { get; set; }

        public ApplicationUser User { get; set; }
        public int UserId { get; set; }

        public List<Event> Events { get; set; }
        public List<Order> Orders { get; set; }
    }
}