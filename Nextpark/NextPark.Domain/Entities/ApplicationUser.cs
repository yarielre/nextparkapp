using Microsoft.AspNetCore.Identity;
using NextPark.Domain.Core;
using System.Collections.Generic;

namespace NextPark.Domain.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser<int>, IBaseEntity
    {
        public string Name { get; set; }
        public string Lastname { get; set; }

        public string Address { get; set; }
        public int Cap { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string Phone { get; set; }

        public string CarPlate { get; set; }

        public string ImageUrl { get; set; }

        public double Balance { get; set; }
        public double Profit { get; set; }

        public List<Parking> Parkings { get; set; }
        public List<Order> Orders { get; set; }
        public List<CarPlate> CarPlates { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<Device> Devices { get; set; }
    }
}
