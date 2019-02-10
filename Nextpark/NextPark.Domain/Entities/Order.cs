using NextPark.Domain.Core;
using NextPark.Enums;
using NextPark.Enums.Enums;
using System;

namespace NextPark.Domain.Entities
{
    public class Order : BaseEntity
    {
        public double Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentCode { get; set; }


        public virtual Parking Parking { get; set; }
        public int ParkingId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public int UserId { get; set; }
    }
}
