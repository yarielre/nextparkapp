using System;
using System.Collections.Generic;
using NextPark.Models;

namespace NextPark.Mobile.UIModels
{
    public class UIParkingModel : ParkingModel
    {
        public UIParkingModel() : base()
        {
            Orders = new List<OrderModel>();
            Events = new List<EventModel>();
        }

        public UIParkingModel(ParkingModel parking) : base()
        {
            Address = parking.Address;
            Cap = parking.Cap;
            CarPlate = parking.CarPlate;
            City = parking.City;
            Id = parking.Id;
            ImageBinary = parking.ImageBinary;
            ImageUrl = parking.ImageUrl;
            Latitude = parking.Latitude;
            Longitude = parking.Longitude;
            PriceMax = parking.PriceMax;
            PriceMin = parking.PriceMin;
            State = parking.State;
            Status = parking.Status;
            UserId = parking.UserId;

            Orders = new List<OrderModel>();
            Events = new List<EventModel>();
        }

        public List<OrderModel> Orders { get; set; }
        public List<EventModel> Events { get; set; }

        public bool isFree()
        {
            foreach (EventModel availability in Events) {
                if ((availability.StartDate <= DateTime.Now) && (availability.EndDate > DateTime.Now)) {
                    foreach (OrderModel order in Orders) {
                        if ((order.StartDate <= DateTime.Now) && (order.EndDate > DateTime.Now)) {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
