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
            if (Status == Enums.Enums.ParkingStatus.Disabled) {
                return false;
            }

            foreach (EventModel availability in Events) {
                if ((availability.StartDate <= DateTime.Now) && (availability.EndDate > DateTime.Now)) {
                    foreach (OrderModel order in Orders) {
                        if ((order.OrderStatus == Enums.OrderStatus.Actived) && (order.StartDate <= DateTime.Now) && (order.EndDate > DateTime.Now)) {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public bool isFree(DateTime start, DateTime end)
        {
            DateTime tempStart = start;
            bool available = false;

            if (Status == Enums.Enums.ParkingStatus.Disabled)
            {
                // Parking disactivated by owner
                return false;
            }

            // Sort events by StartDate
            Events.Sort((a, b) => (a.StartDate.CompareTo(b.StartDate)));

            foreach (EventModel availability in Events)
            {
                if ((availability.StartDate <= tempStart) && (availability.EndDate > tempStart))
                {
                    if (availability.EndDate >= end)
                    {
                        available = true;
                        break;
                    } else {
                        // Search for contiguous next event
                        tempStart = availability.EndDate.AddMinutes(1);
                    }
                }
            }

            if (available == true) {
                foreach (OrderModel order in Orders)
                {
                    if ((order.OrderStatus == Enums.OrderStatus.Actived) && (order.EndDate > start) && (order.StartDate < end))
                    {
                        // order already present (overlap at start, inside actual request, overlap at end)
                        return false;
                    }
                }
                // No previous orders found, parking id free
                return true;
            }

            // Parking not available
            return false;
        }

        // Returns how much time the parking is available from parameter start
        public TimeSpan GetAvailableTime(DateTime start)
        {
            DateTime tempStart = start;
            DateTime tempEnd = start.AddMinutes(1);
            bool available = false;

            if (Status == Enums.Enums.ParkingStatus.Disabled)
            {
                // Parking disactivated by owner
                return TimeSpan.FromMinutes(0);
            }

            // Sort events by StartDate
            Events.Sort((a, b) => (a.StartDate.CompareTo(b.StartDate)));

            foreach (EventModel availability in Events)
            {
                if ((availability.StartDate <= tempEnd) && (availability.EndDate > tempEnd))
                {
                    available = true;
                    tempEnd = availability.EndDate.AddMinutes(1);
                }
            }
            tempEnd = tempEnd.AddMinutes(-1);

            if (available)
            {
                foreach (OrderModel order in Orders)
                {
                    if ((order.OrderStatus == Enums.OrderStatus.Actived) && (order.EndDate > start) && (order.StartDate < tempEnd))
                    {
                        // order present (end after availability request and start before temporary end)
                        tempEnd = order.StartDate;
                    }
                }
            }

            return tempEnd - start;
        }
    }
}
