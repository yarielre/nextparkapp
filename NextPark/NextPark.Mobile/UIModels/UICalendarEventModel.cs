using System;
using System.ComponentModel;
using System.Windows.Input;
using NextPark.Models;
using Xamarin.Forms;

namespace NextPark.Mobile.UIModels
{
    public class UICalendarEventModel : INotifyPropertyChanged
    {
        public string Text { get; set; }
        public Color TextColor { get; set; }
        public int StartSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public Color EventColor { get; set; }
        public Constraint yConstPosition { get; set; }
        public Constraint xConstPosition { get; set; }
        public int Index { get; set; }
        public ICommand OnEventTap { get; set; }
        public OrderModel Order { get; set; }
        public EventModel Event { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        // METHODS
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
