using NextPark.Enums;
using NextPark.Enums.Enums;
using System;
using System.Collections.Generic;

namespace NextPark.Models
{
    public class EventModel : BaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid RepetitionId { get; set; }
        public DateTime RepetitionEndDate { get; set; }
        public RepetitionType RepetitionType { get; set; }

        public int ParkingId { get; set; }
    }
}