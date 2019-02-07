using System;
using System.Collections.Generic;
using NextPark.Models;

namespace NextPark.Mobile.UIModels
{
    public class UIParkingModel : ParkingModel
    {
        public UIParkingModel() : base()
        {
        }

        public List<OrderModel> Orders { get; set; }
        public List<EventModel> Events { get; set; }

        public bool isFree()
        {
            return true;
        }
    }
}
