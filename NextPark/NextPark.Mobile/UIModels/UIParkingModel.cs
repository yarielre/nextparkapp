using System;
using System.Collections.Generic;
using System.ComponentModel;
using NextPark.Models;

namespace NextPark.Mobile.UIModels
{
    public class UIParkingModel : ParkingModel, INotifyPropertyChanged
    {
        public UIParkingModel() : base()
        {
        }

        public List<OrderModel> Orders { get; set; }
        public List<EventModel> Events { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // METHODS
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool isFree()
        {
            return true;
        }
    }
}
