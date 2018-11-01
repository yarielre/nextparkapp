using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inside.Xamarin.Services;
using Inside.Xamarin.ViewModels;
using Xamarin.Forms;

namespace Inside.Xamarin.Views.Master
{
    public partial class MasterView : MasterDetailPage
    {
        public MasterView()
        {
            InitializeComponent();
            OnInitAsync();
        }

        private async void OnInitAsync()
        {
            await MainViewModel.GetInstance().RestoreLoginData();
        }
        //private void OnInit()
        //{
        //    //Esperas a que este metodo termine para continuar. 
        //    //Utilizando el wait garantizas que antes de continuar la ejecucion de este thread 
        //    //se executen todos los threads detivados de esta linea.
        //    MainViewModel.GetInstance().RestoreLoginData().Wait();
        //}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.Navigator = this.Navigator;
            App.Master = this;

        }
    }
}
