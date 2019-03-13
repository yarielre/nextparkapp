using NextPark.Enums;
using NextPark.Enums.Enums;
using System;

namespace NextPark.Models
{
    public class OrderModel : BaseModel
    {
        public double Price { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }
        public string PaymentCode { get; set; }

        public int ParkingId { get; set; }

        public int UserId { get; set; }

        public string CarPlate { get; set; }
     
    }
}
