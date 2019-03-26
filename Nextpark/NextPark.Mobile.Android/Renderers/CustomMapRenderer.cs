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
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Support.Annotation;
using Android.Util;

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
        }

        protected override void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            if (_map != null)
            {
                _map.MapClick += googleMap_MapClick;
                _map.MarkerClick += OnMarkerClick;
                ((CustomMap)Element).OnMapReady();
                _map.UiSettings.MyLocationButtonEnabled = true;
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

            var resourceId = Resources.GetIdentifier(customPin.Icon, "drawable", "com.wisegar.nextpark");

            float density = Context.Resources.DisplayMetrics.Density;
            int dpValue = (int)(50.0 * density); // 50px in dips
            Bitmap image = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeResource(Resources, resourceId), dpValue, dpValue, true);
            BitmapDescriptor bitmapResource = BitmapDescriptorFactory.FromBitmap(image);
            marker.SetIcon(bitmapResource);

            return marker;
        }
    }
}