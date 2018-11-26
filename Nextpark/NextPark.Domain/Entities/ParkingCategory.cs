using NextPark.Domain.Core;
using System.Collections.Generic;

namespace NextPark.Domain.Entities
{
    public class ParkingCategory : BaseEntity
    {
        public string Category { get; set; }
        public double MonthPrice { get; set; }
        public double HourPrice { get; set; }

        public List<Parking> Parkings { get; set; }
    }
}