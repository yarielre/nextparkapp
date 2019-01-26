using System;
using NextPark.Domain.Core;
using NextPark.Enums.Enums;

namespace NextPark.Domain.Entities
{
    public class Event : BaseEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid RepetitionId { get; set; }
        public DateTime RepetitionEndDate { get; set; }
        public RepetitionType RepetitionType { get; set; }

        public virtual Parking Parking { get; set; }
        public int ParkingId { get; set; }
    }
}