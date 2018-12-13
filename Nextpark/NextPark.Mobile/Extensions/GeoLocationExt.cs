using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Mobile.Extensions
{
    public static class GeoLocationExt
    {
        public static Xamarin.Forms.Maps.Position ToXamMapPosition(this Plugin.Geolocator.Abstractions.Position position)
        {
            if (position == null)
            {
                return new Xamarin.Forms.Maps.Position();
            }
            return new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
        }

    }
}
