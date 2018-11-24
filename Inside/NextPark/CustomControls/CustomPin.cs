using NextPark.Models;
using Xamarin.Forms.Maps;

namespace Inside.Xamarin.CustomControls
{
    public class CustomPin : Pin
    {
        public ParkingModel Parking { get; set; }
        public string Icon { get; set; }
    }
}