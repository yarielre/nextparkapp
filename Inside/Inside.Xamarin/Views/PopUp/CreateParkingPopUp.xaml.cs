using Inside.Xamarin.Services;
using Inside.Xamarin.ViewModels;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;
using Position = Xamarin.Forms.Maps.Position;

namespace Inside.Xamarin.Views.PopUp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateParkingPopUp : PopupPage
	{
	    private readonly global::Xamarin.Forms.Maps.Position _coordenate;
		public CreateParkingPopUp (Position position)
		{
			InitializeComponent ();
            _coordenate = position;
		}
	    public CreateParkingPopUp()
	    {
	        InitializeComponent();
	    }


        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }

        private async void ButtonYes_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
            MainViewModel.GetInstance().ParkingCreate = new ParkingCreateViewModel(this._coordenate);
            await NavigationService.GetInstance().NavigateOnMaster(Pages.ParkingCreate);
        }

        private async void ButtonNo_OnClicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}
	
