using System;
using System.Collections.Generic;
using System.Text;
using Inside.Domain.Enum;

namespace Inside.Domain.Models
{
   public class OrderModel:BaseModel
    {
        public double Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int ParkingId { get; set; }
        public int UserId { get; set; }
    }
}
