using Android.App;
using Android.Content.PM;
using Android.OS;
using Acr.UserDialogs;
using Plugin.InAppBilling;
using Android.Content;
using Xamarin.Forms;
using NextPark.Mobile.ViewModels;

namespace NextPark.Mobile.Droid
{
    [Activity(Label = "NextPark.Mobile", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::CarouselView.FormsPlugin.Android.CarouselViewRenderer.Init();

            UserDialogs.Init(this);

            global::Xamarin.FormsMaps.Init(this, savedInstanceState);

            LoadApplication(new App());
        }
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InAppBillingImplementation.HandleActivityResult(requestCode, resultCode, data);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected void OnCloseAppRequested()
        {
            Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("Conferma chiusura");
            alert.SetMessage("Chiudere l'applicazione?");
            alert.SetButton("OK", (c, ev) =>
            {
                base.OnBackPressed();
            });
            alert.SetButton2("ANNULLA", (c, ev) => { });
            alert.Show();
        }
        public override void OnBackPressed()
        {
            Page currentPage = Xamarin.Forms.Application.Current.MainPage;
            if (currentPage != null) {
                if (currentPage.BindingContext is BaseViewModel bvm)
                {
                    if (bvm.BackButtonPressed()) {
                        OnCloseAppRequested();
                    }
                }
            }
        }
    }
}