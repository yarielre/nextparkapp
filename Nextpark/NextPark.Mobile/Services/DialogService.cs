using System.Threading.Tasks;
using Acr.UserDialogs;

namespace NextPark.Mobile.Services
{
    public class DialogService : IDialogService
    {
        private readonly ILocalizationService _localizationService;
        public DialogService(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        public Task ShowAlert(string title, string message)
        {
            return UserDialogs.Instance.AlertAsync(message, title, "Accettare");
        }
        public async Task<bool> ShowConfirmAlert(string title, string message)
        {
            var result = await UserDialogs.Instance.ConfirmAsync(message, title, "Accettare", "Cancellare");
            return result;
        }
        public Task ShowErrorAlert(string message)
        {
            return UserDialogs.Instance.AlertAsync(message, "Errore", "Accettare");
        }

        public void ShowToast(string message)
        {
            UserDialogs.Instance.Toast(message);
        }
    }
}
