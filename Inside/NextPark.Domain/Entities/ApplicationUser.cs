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
        public string State { get; set; }
        public string CarPlate { get; set; }

        public double Coins { get; set; }
        public List<Parking> Parkings { get; set; }
        public List<Order> Orders { get; set; }
    }
}
