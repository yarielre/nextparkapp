using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using NextPark.Mobile.CustomControls;
using NextPark.Mobile.Droid.Renderers;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using static Android.Gms.Maps.GoogleMap;
using System.ComponentModel;

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
        private bool _mapReady = false;

        // We use a native google map for Android
        private GoogleMap _map;


        public CustomMapRenderer(Context context) : base(context)
        {
            _mapReady = false;
        }

        protected override void OnMapReady(GoogleMap map)
        {
            if (_mapReady == true) return;

            _map = map;
            if (_map != null)
            {
                _mapReady = true;
                _map.MapClick += googleMap_MapClick;
                _map.MarkerClick += OnMarkerClick;
                ((CustomMap)Element).OnMapReady();
                _map.CameraChange += Map_CameraChange;
                _map.UiSettings.ZoomControlsEnabled = false;
                _map.UiSettings.MyLocationButtonEnabled = false;
            }
        }

        private void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var pos = new Position(e.Marker.Position.Latitude, e.Marker.Position.Longitude);
            var pin = _formsMap.Pins.First(p => p.Position == pos);
            var customPin = pin as CustomPin;
            if (customPin != null)
            {
                ((CustomMap)Element).OnPinTap(customPin.Parking);
            }
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
                _formsMap = (CustomMap)e.NewElement;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == CustomMap.ShowUserEnableProperty.PropertyName) {
                UpdateShowUserEnable();
            }
        }

        private void UpdateShowUserEnable()
        {
            if (_formsMap.ShowUserEnable == true)
            {
                _map.MyLocationEnabled = true;
            } else
            {
                _map.MyLocationEnabled = false;
            }
        }


        private void googleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            ((CustomMap)Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            var customPin = pin as CustomPin;
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);

            var resourceId = Resources.GetIdentifier(customPin.Icon, "drawable", Android.App.Application.Context.PackageName);

            float density = Context.Resources.DisplayMetrics.Density;
            int dpValue = (int)(50.0 * density); // 50px in dips
            Bitmap image = Bitmap.CreateScaledBitmap(BitmapFactory.DecodeResource(Resources, resourceId), dpValue, dpValue, false);
            BitmapDescriptor bitmapResource = BitmapDescriptorFactory.FromBitmap(image);
            marker.SetIcon(bitmapResource);

            return marker;
        }


        /// <summary>
        /// this event notifies camerachange to viewModel by using OnMapMoved delegate
        private void Map_CameraChange(object sender, CameraChangeEventArgs e)
        {
            if (((CustomMap)this.Element) != null && _map != null)
            {
                var centerPosition = _map.Projection.VisibleRegion.LatLngBounds.Center;
                var nearPosition = _map.Projection.VisibleRegion.NearRight;
                if (centerPosition == null)
                {
                    centerPosition = nearPosition;
                }
                ((CustomMap)Element).OnMapMoved(new Position(centerPosition.Latitude, centerPosition.Longitude));
            }
        }

    }
}
