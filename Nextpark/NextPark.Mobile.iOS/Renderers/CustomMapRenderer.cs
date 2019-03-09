using System.Linq;
using Inside.Xamarin.iOS.Renderers;
using MapKit;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using NextPark.Mobile.CustomControls;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Maps;
using System.Collections.Generic;
using CoreGraphics;
using System;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]

namespace Inside.Xamarin.iOS.Renderers
{
    public class CustomMKAnnotationView : MKAnnotationView
    {
        public string Type { get; set; }

        public string Url { get; set; }

        public CustomMKAnnotationView(IMKAnnotation annotation, string id)
            : base(annotation, id)
        {
        }
    }

    public class CustomMapRenderer : MapRenderer
    {
        private readonly UITapGestureRecognizer _tapRecogniser;
        private CustomMap _formsMap;
        private static string NextParkAnnotationType = "NextParkAnnotation";

        private UIView customPinView;

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
                    nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                    nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                    nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
                }
            }

            if (e.NewElement != null)
            {
                _formsMap = (CustomMap) e.NewElement;
                var nativeMap = Control as MKMapView;

                if (nativeMap != null)
                {
                    nativeMap.AddGestureRecognizer(_tapRecogniser);
                    nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                    nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                    nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;


                    ((CustomMap)Element).OnMapReady();
                }
            }
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;
            // My Location is excluded
            if (annotation is MKUserLocation)
                return null;

            // Get Custom pin
            var customPin = GetCustomPin(annotation);
            if (customPin == null)
            {
                // Pin not found
                return null;
            }

            // Use icon name to identify reusable annotation
            annotationView = mapView.DequeueReusableAnnotation(customPin.Icon);
            if (annotationView == null)
            {
                // If not yet created, create a new annotation with icon name as identifier
                annotationView = new CustomMKAnnotationView(annotation, customPin.Icon);//customPin.Id.ToString());
                annotationView.Image = UIImage.FromFile(customPin.Icon+".png").Scale(new CGSize(50.0, 50.0));
                annotationView.CalloutOffset = new CGPoint(0, 0);
                //annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile(customPin.Icon+".png"));
                //annotationView.LeftCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);
                ((CustomMKAnnotationView)annotationView).Type = NextParkAnnotationType; // Use NextParkAnnotation to identify the NextPark annotation type

            }

            annotationView.CanShowCallout = true;

            return annotationView;
        }

        void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            if (!string.IsNullOrWhiteSpace(customView.Url))
            {
                UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
            }
        }

        void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var customView = (CustomMKAnnotationView)GetViewForAnnotation((MKMapView)sender, e.View.Annotation);
            customPinView = new UIView();

            if ((customView != null) && (customView.Type.Equals(NextParkAnnotationType)))
            {
                customPinView.Frame = new CGRect(0, 0, 200, 84);
                //var image = new UIImageView(new CGRect(0, 0, 200, 84));
                //image.Image = UIImage.FromFile("xamarin.png");
                //customPinView.AddSubview(image);
                customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
                e.View.AddSubview(customPinView);

                var customPin = GetCustomPin(e.View.Annotation);
                ((CustomMap)Element).OnPinTap(customPin.Parking);
            }
            var customPin = GetCustomPin(e.View.Annotation);
            if (customPin != null)
            {
                // My Location excluded
                ((CustomMap)Element).OnPinTap(customPin.Parking);
            }
        }

        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if ((!e.View.Selected) && (customPinView!= null))
            {
                customPinView.RemoveFromSuperview();
                customPinView.Dispose();
                customPinView = null;
            }
        }
        private CustomPin GetCustomPin(IMKAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);

            foreach (CustomPin pin  in _formsMap.Pins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }

    }
}
