using Inside.Xamarin.CustomControls;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using Inside.Xamarin.ViewModels;
using Inside.Xamarin.Views.PopUp;
using NextPark.Models;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;

namespace Inside.Xamarin.Views.Home
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly GeolocatorService _geolocatorService;
        private readonly ApiService _insideApi;
        private ObservableCollection<ParkingModel> _parkings;
        public event PropertyChangedEventHandler PropertyChanged;
        protected override void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public bool tappedAlready { get; set; }
        public HomePage()
        {
            _insideApi = new ApiService(); //TODO: Use DI
            _geolocatorService = new GeolocatorService(); //TODO: Use DI
            InitializeComponent();
            OnInit();
           // this.tappedAlready = false;
            // This is a callback to get a parking who was created in create parkingviewmodel and show it in the map.
            MessagingCenter.Subscribe<ParkingModel>(this, Messages.NewParkingCreated, newParking =>
            {
                var pos = new Position(double.Parse(newParking.Latitude), double.Parse(newParking.Longitude));
                CreatePin(pos, newParking);
            });

               MessagingCenter.Subscribe<ParkingModel>(this, Messages.ParkingEdited, EditParking);
        }

        public ObservableCollection<ParkingModel> Parkings
        {
            get => _parkings;
            set
            {
                _parkings = value;
                OnPropertyChanged("Parkings");
            }
        }

        private void OnInit()
        {
            MyMap.Tapped += MyMap_Tapped;
            MyMap.PinTapped += MyMap_PinTapped;
            MyMap.MapReady += MyMap_MapReady;
        }

        private void MyMap_MapReady(object sender, System.EventArgs e)
        {
            GetParkings();
            InitLocation();
        }

        private async void InitLocation() {

            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need location", "Gunna need that location", "OK");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Plugin.Permissions.Abstractions.Permission.Location))
                        status = results[Permission.Location];
                    
                }

                if (status == PermissionStatus.Granted)
                {
                    if (!CrossGeolocator.IsSupported)
                    {
                        await DisplayAlert("Geolocation", "Geolocation service not supported!", "Ok");
                        return;
                    }

                    if (!CrossGeolocator.Current.IsGeolocationAvailable)
                    {
                        await DisplayAlert("Geolocation", "Geolocation service not available!", "Ok");
                        return;
                    }
                    if (!CrossGeolocator.Current.IsGeolocationEnabled)
                    {
                        await DisplayAlert("Geolocation", "Geolocation service disabled!", "Ok");
                        return;
                    }

                    var lastKnowPosition = await CrossGeolocator.Current.GetLastKnownLocationAsync();

                    var position = new Position(lastKnowPosition.Latitude, lastKnowPosition.Longitude);
                    MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));

                    CrossGeolocator.Current.PositionChanged += Current_PositionChanged;
                }

            }
            catch (Exception ex)   {  }
        }

        private void Current_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            var position = new Position(e.Position.Latitude, e.Position.Longitude);
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(1)));
        }

        private async void MyMap_PinTapped(object sender, PinTapEventArgs e)
        {
            var parking = e.Parking;

            if (parking.UserId == MainViewModel.GetInstance().CurrentUser.Id)
            {

                MainViewModel.GetInstance().ParkingEdit = new ParkingEditViewModel(parking);
                await NavigationService.GetInstance().NavigateOnMaster(Pages.ParkingEdit);
            }
            else
            {
                if (parking.ParkingType.Type == "By Hours")
                {
                    MainViewModel.GetInstance().ParkingRent = new ParkingRentViewModel(parking, true);
                    await NavigationService.GetInstance().NavigateOnMaster(Pages.ParkingRentView);
                }
                else
                {
                    MainViewModel.GetInstance().ParkingRent = new ParkingRentViewModel(parking, false);
                    await NavigationService.GetInstance().NavigateOnMaster(Pages.ParkingRentView);
                }
            }

        }

        private void MyMap_Tapped(object sender, MapTapEventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new CreateParkingPopUp(e.Position));
            //if (this.tappedAlready)
            //{
            //    PopupNavigation.Instance.PushAsync(new CreateParkingPopUp(e.Position));
            //    return;
            //}

            //if (Parkings.Count > 0)
            //{
            //    foreach (var parking in Parkings)
            //    {
            //        try
            //        {
            //            var position = new Position(double.Parse(parking.Latitude), double.Parse(parking.Longitude));
            //            CreatePin(position, parking);
            //        }
            //        catch
            //        {
            //            //Error on pin creation. Usually parsing coordinates!
            //            continue;
            //        }
            //    }
            //    this.tappedAlready = true;
            //}

            //   InitLocation();

        }

        private async void GetParkings()
        {
            var response = await _insideApi.GetAllParkings(HostSetting.ParkingEndPoint);

            if (response.Result == null) return;

            Parkings = new ObservableCollection<ParkingModel>((List<ParkingModel>)response.Result);

            if (Parkings.Count > 0)
            {
                foreach (var parking in Parkings)
                {
                    try
                    {
                        var position = new Position(double.Parse(parking.Latitude), double.Parse(parking.Longitude));
                        CreatePin(position, parking);
                    }
                    catch
                    {
                        //Error on pin creation. Usually parsing coordinates!
                        continue;
                    }
                }
             //   this.tappedAlready = true;
            }
        }

        private void EditParking(ParkingModel parking)
        {
            var parkingEdited = Parkings.Single(p => p.Id == parking.Id);
            var pos = Parkings.IndexOf(parkingEdited);
            Parkings[pos] = parking;
        }
        private void CreatePin(Position position, ParkingModel parking)
        {
            var pin = new CustomPin
            {
                Id = parking.Id,
                Parking = parking,
                Type = PinType.Place,
                Position = position,
                Label = "custom pin",
                Address = "custom detail info",
                Icon = "ic_location_green"
            };
            if (parking.ParkingCategory.Category == "Basic")
            {
                pin.Icon = "ic_location_black";
            }
            if (parking.IsRented)
            {
                pin.Icon = "ic_location_rented";
            }

            MyMap.Pins.Add(pin);
        }
    }
}