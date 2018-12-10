using System;
using Android.Content;
using Android.Gms.Maps;
using System.Linq;
using Android.Gms.Maps.Model;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using NextPark.Mobile.CustomControls;
using NextPark.Mobile.Droid.Renderers;
using NextPark.Models;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]

namespace NextPark.Mobile.Droid.Renderers
{
    /// <summary>
    ///     Renderer for the xamarin map.
    ///     Enable user to get a position by taping on the map.
    /// </summary>
    public class CustomMapRenderer : MapRenderer, IOnMapReadyCallback
    {
        private CustomMap _formsMap;

        // We use a native google map for Android
        private GoogleMap _map;


        public CustomMapRenderer(Context context) : base(context)
        {
           AndroidMessageSubscriber();
        }

        private void AndroidMessageSubscriber()
        {
            //MessagingCenter.Subscribe<ParkingModel>(this, Messages.ChangeIconAfterRent, parking =>
            //{
            //    var pos = new Position(Double.Parse(parking.Latitude), Double.Parse(parking.Longitude));
            //    var pin = _formsMap.Pins.First(p => p.Position == pos);
            //    var customPin = pin as CustomPin;
            //    customPin.Icon = "ic_location_rented";
            //    _formsMap.Pins.Remove(pin);
            //    _formsMap.Pins.Add(customPin);
            //});
            //MessagingCenter.Subscribe<ParkingModel>(this, Messages.ChangeIconAfterTerminateRent, parking =>
            //{
            //    var pos = new Position(Double.Parse(parking.Latitude), Double.Parse(parking.Longitude));
            //    var pin = _formsMap.Pins.First(p => p.Position == pos);
            //    var customPin = pin as CustomPin;
            //    if (parking.ParkingCategory.Category=="Basic")
            //    {
            //        customPin.Icon = "ic_location_black";
            //    }
            //    else
            //    {
            //        customPin.Icon = "ic_location_green";
            //    }
            //    _formsMap.Pins.Remove(pin);
            //    _formsMap.Pins.Add(customPin);
            //});
        }

        protected override void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            if (_map != null)
            {
                _map.MapClick += googleMap_MapClick;
                _map.MarkerClick += OnMarkerClick;
                ((CustomMap)Element).OnMapReady();
            }
        }

        private void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var pos = new Position(e.Marker.Position.Latitude, e.Marker.Position.Longitude);
            var pin = _formsMap.Pins.First(p => p.Position == pos);
            var customPin = pin as CustomPin;
            if (customPin != null) ((CustomMap) Element).OnPinTap(customPin.Parking);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                _map.MapClick -= googleMap_MapClick;
                _map.MarkerClick -= OnMarkerClick;
            }

            if (e.NewElement != null)
            {
                _formsMap = (CustomMap) e.NewElement;
                Control.GetMapAsync(this);
            }
        }

        /* private void CustomPins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
         {
             _customPins = (ObservableCollection<CustomPin>)sender;
             _map.Clear();
             foreach (var pin in _customPins)
             {
                 var marker = new MarkerOptions();
                 marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                 marker.SetTitle(pin.Label);
                 marker.SetSnippet(pin.Address);
                // marker.SetIcon(BitmapDescriptorFactory.FromResource(Resource.Drawable.pin));

                 _map.AddMarker(marker);
             }
         }*/

        private void googleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            ((CustomMap) Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
        }
        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            var customPin = pin as CustomPin;
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.SetIcon(BitmapDescriptorFactory.FromResource(Resources.GetIdentifier(customPin.Icon, "drawable", "com.wisegar.inside")));
            return marker;
        }
    }
}