using Inside.Domain.Enum;
using System;
using Inside.Domain.Core;

namespace Inside.Domain.Entities
{
    public class Order:BaseEntity
    {
        //Cambiar los TimeSpan a DateTime
        public double Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }


        public virtual Parking Parking { get; set; }
        public int ParkingId { get; set; }

        public int UserId { get; set; }
    }
}
