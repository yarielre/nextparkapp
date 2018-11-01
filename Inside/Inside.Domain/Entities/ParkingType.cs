using System.Collections.Generic;
using Inside.Domain.Core;

namespace Inside.Domain.Entities
{
    public class ParkingType:BaseEntity
    {
        public string Type { get; set; }
        public List<Parking> Parkings { get; set; }
    }
}