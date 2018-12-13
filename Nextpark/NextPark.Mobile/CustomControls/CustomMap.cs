﻿using System;
using NextPark.Models;
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
    }
    /// <summary>
    /// Event args used with maps, when the user tap on it
    /// </summary>
    public class MapTapEventArgs : EventArgs
    {
        public Position Position { get; set; }
    }
    public class PinTapEventArgs : EventArgs
    {
        public ParkingModel Parking { get; set; }
    }
}