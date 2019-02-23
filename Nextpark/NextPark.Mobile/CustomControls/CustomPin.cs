using NextPark.Models;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.CustomControls
{
    public class CustomPin : Pin
    {
        public ParkingModel Parking { get; set; }
        public string Icon { get; set; }
    }
}