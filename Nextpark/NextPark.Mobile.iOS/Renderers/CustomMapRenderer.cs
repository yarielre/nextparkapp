﻿using System.Linq;
using Inside.Xamarin.iOS.Renderers;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using NextPark.Mobile.CustomControls;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Maps;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]

namespace Inside.Xamarin.iOS.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        private readonly UITapGestureRecognizer _tapRecogniser;
        private CustomMap _formsMap;


        public CustomMapRenderer()
        {
            _tapRecogniser = new UITapGestureRecognizer(OnTap)
            {
                NumberOfTapsRequired = 1,
                NumberOfTouchesRequired = 1
            };
        }

        private void OnTap(UITapGestureRecognizer recognizer)
        {
            var cgPoint = recognizer.LocationInView(Control);

            var location = ((MKMapView) Control).ConvertPoint(cgPoint, Control);

            ((CustomMap) Element).OnTap(new Position(location.Latitude, location.Longitude));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var nativeMap = Control as MKMapView;
                if (nativeMap != null)
                {
                    nativeMap.RemoveGestureRecognizer(_tapRecogniser);
                    nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                }
            }

            if (e.NewElement != null)
            {
                _formsMap = (CustomMap) e.NewElement;
                var nativeMap = Control as MKMapView;

                if (nativeMap != null)
                {
                    nativeMap.AddGestureRecognizer(_tapRecogniser);
                    nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                }
            }
        }

        private void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var pos = new Position(e.View.Annotation.Coordinate.Latitude, e.View.Annotation.Coordinate.Longitude);
            var pin = _formsMap.Pins.First(p => p.Position == pos);
            var customPin = pin as CustomPin;
            if (customPin != null) ((CustomMap)Element).OnPinTap(customPin.Parking);
        }

    }
}