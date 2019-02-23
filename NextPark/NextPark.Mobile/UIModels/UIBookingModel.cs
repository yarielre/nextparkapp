using System;
using System.ComponentModel;
using System.Windows.Input;
using NextPark.Models;

namespace NextPark.Mobile.UIModels
{
    public class UIBookingModel : OrderModel, INotifyPropertyChanged
    {
        private int uid;
        public int UID
        {
            get { return uid; }
            set { uid = value; }
        }

        private int index;
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        private string address;
        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string cap;
        public string Cap
        {
            get { return cap; }
            set { cap = value; }
        }

        private string city;
        public string City
        {
            get { return city; }
            set { city = value; }
        }

        private string time;
        public string Time
        {
            get { return time; }
            set { time = value; }
        }

        private ICommand onBookingTap;
        public ICommand OnBookingTap
        {
            get { return onBookingTap; }
            set { onBookingTap = value; }
        }

        private ICommand onBookingDel;
        public ICommand OnBookingDel
        {
            get { return onBookingDel; }
            set { onBookingDel = value; }
        }

        public ParkingModel Parking { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // METHODS
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
