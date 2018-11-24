using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Inside.Xamarin.Annotations;
using Inside.Xamarin.Helpers;
using Inside.Xamarin.Services;
using Xamarin.Forms;

namespace Inside.Xamarin.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private ApiService _apiService;

        public int Id { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        public ApiService InsideApi => _apiService ?? (_apiService = new ApiService());

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return;
            }
            backingField = value;
            OnPropertyChanged(propertyName);
        }

        protected virtual void CurrentPageOnAppearing(object sender, System.EventArgs eventArgs) { }

        protected virtual void CurrentPageOnDisappearing(object sender, System.EventArgs eventArgs) { }

    }
}