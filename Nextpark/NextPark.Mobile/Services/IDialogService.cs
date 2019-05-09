using System;
using System.Threading.Tasks;

namespace NextPark.Mobile.Services
{
    public interface IDialogService
    {
        Task ShowAlert(string title, string message);
        Task<bool> ShowConfirmAlert(string title, string message);
        Task ShowErrorAlert(string message);
        void ShowToast(string message);
        void ShowToast(string message, TimeSpan dismissTimer);
    }
}