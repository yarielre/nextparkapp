using System.Collections.Generic;
using Inside.Domain.Core;

namespace Inside.Domain.Entities
{
    public class ParkingCategory:BaseEntity
    {
        public string Category { get; set; }
        public double MonthPrice { get; set; }
        public double HourPrice { get; set; }

        public List<Parking> Parkings { get; set; }
    }
}