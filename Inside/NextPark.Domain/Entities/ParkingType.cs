using NextPark.Domain.Core;
using System.Collections.Generic;

namespace NextPark.Domain.Entities
{
    public class ParkingType : BaseEntity
    {
        public string Type { get; set; }
        public List<Parking> Parkings { get; set; }
    }
}