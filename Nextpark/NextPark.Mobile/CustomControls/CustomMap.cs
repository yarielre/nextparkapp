using System;
using NextPark.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace NextPark.Mobile.CustomControls
{
    public class CustomMap : Map
    {
        /// <summary>
        /// Event thrown when the user taps on the map
        /// </summary>
        public event EventHandler<MapTapEventArgs> Tapped;
        public event EventHandler<PinTapEventArgs> PinTapped;
        public event EventHandler MapReady;
        public event EventHandler<MapMovedArgs> MapMoved;
        
        public static readonly BindableProperty ShowUserEnableProperty = BindableProperty.Create(
            propertyName: "ShowUserEnable",
            returnType: typeof(bool),
            declaringType: typeof(CustomMap),
            defaultValue: false,
            defaultBindingMode: BindingMode.TwoWay
            );

        public bool ShowUserEnable
        {
            get { return (bool)GetValue(ShowUserEnableProperty); }
            set { SetValue(ShowUserEnableProperty, value); }
        }

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public CustomMap() : base()
        {

        }

        /// <summary>
        /// Constructor that takes a region
        /// </summary>
        /// <param name="region"></param>
        public CustomMap(MapSpan region)
            : base(region)
        {

        }

        #endregion

        public void OnTap(Position coordinate)
        {
            OnTap(new MapTapEventArgs { Position = coordinate });
        }
        public void OnMapReady()
        {
            OnMapReady(new EventArgs());
        }

        public void OnPinTap(ParkingModel parking)
        {
            OnPinTap(new PinTapEventArgs{ Parking = parking });
        }

        public void OnMapMoved(Position coordinate) 
        {
            OnMapMoved(new MapMovedArgs { Position = coordinate });
        }

        protected virtual void OnTap(MapTapEventArgs e)
        {
            Tapped?.Invoke(this, e);
        }
        protected virtual void OnPinTap(PinTapEventArgs e)
        {
            PinTapped?.Invoke(this, e);
        }
        protected virtual void OnMapReady(EventArgs e)
        {
            MapReady?.Invoke(this, e);
        }
        protected virtual void OnMapMoved(MapMovedArgs e)
        {
            MapMoved?.Invoke(this, e);
        }
    }
    /// <summary>
    /// Event args used with maps, when the user tap on it
    /// </summary>
    public class MapTapEventArgs : EventArgs
    {
        public Position Position { get; set; }
    }
    public class MapMovedArgs : EventArgs
    {
        public Position Position { get; set; }
    }
    public class PinTapEventArgs : EventArgs
    {
        public ParkingModel Parking { get; set; }
    }
}
