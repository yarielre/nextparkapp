using System.Threading.Tasks;
using Acr.UserDialogs;

namespace NextPark.Mobile.Core.Services
{
    public class DialogService
    {

        //public Task ShowAlert(string title, string message)
        //{
        //   return UserDialogs.Instance.AlertAsync(message, title, Languages.Accept);

        //    //await Application.Current.MainPage.DisplayAlert(title, message, Languages.Accept);
        //}
        //public async Task<bool> ShowConfirmAlert(string title, string message)
        //{
        //    var result = await UserDialogs.Instance.ConfirmAsync(message, title, Languages.Accept, Languages.Cancel);
        //    return result;
        //    //return await Application.Current.MainPage.DisplayAlert(title, message, Languages.Cancel, Languages.Accept);
        //}
        //public Task ShowErrorAlert(string message)
        //{
        //    return UserDialogs.Instance.AlertAsync(message, Languages.Error, Languages.Accept);
        //    //  return await App.Navigator.CurrentPage.DisplayAlert(title, message, Languages.GeneralAccept, Languages.GeneralCancel);
        //    //await Application.Current.MainPage.DisplayAlert(Languages.Error, message, Languages.Accept);
        //}
       
         public void ShowToast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }

        public DialogService()
        {
            Instance = this;
        }
        //TODO Instance is always null
        private static DialogService Instance
        {
            get;
            set;
        }
        public static DialogService GetInstance()
        {
            return Instance ?? new DialogService();
        }
    }
}
