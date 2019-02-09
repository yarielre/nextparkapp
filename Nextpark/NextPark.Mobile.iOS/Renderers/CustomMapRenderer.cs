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
        public string Id { get; set; }

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
        private static string annotationId = "NextParkAnnotation";

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

        //private void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        //{
        //    var pos = new Position(e.View.Annotation.Coordinate.Latitude, e.View.Annotation.Coordinate.Longitude);
        //    var pin = _formsMap.Pins.First(p => p.Position == pos);
        //    var customPin = pin as CustomPin;
        //    if (customPin != null) ((CustomMap)Element).OnPinTap(customPin.Parking);
        //}

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

     
            if (annotation is MKUserLocation)
                return null;
    
            var customPin = GetCustomPin(annotation);

            if (customPin == null)
            {
                return null;
            }

            annotationView = mapView.DequeueReusableAnnotation(annotationId);

            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, annotationId);//customPin.Id.ToString());

                annotationView.Image = UIImage.FromFile(customPin.Icon+".png").Scale(new CGSize(50.0, 50.0));
                annotationView.CalloutOffset = new CGPoint(0, 0);
                annotationView.LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile(customPin.Icon+".png"));
                annotationView.RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure);

                ((CustomMKAnnotationView)annotationView).Id = annotationId;
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
            //var customView = e.View as CustomMKAnnotationView;
            var customView = (CustomMKAnnotationView)GetViewForAnnotation((MKMapView)sender, e.View.Annotation);
            customPinView = new UIView();

            if (customView.Id == "NextParkAnnotation")
            {
                customPinView.Frame = new CGRect(0, 0, 200, 84);
                var image = new UIImageView(new CGRect(0, 0, 200, 84));
                image.Image = UIImage.FromFile("xamarin.png");
                customPinView.AddSubview(image);
                customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
                e.View.AddSubview(customPinView);
            }
            var customPin = GetCustomPin(e.View.Annotation);
            ((CustomMap)Element).OnPinTap(customPin.Parking);
        }

        void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (!e.View.Selected)
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