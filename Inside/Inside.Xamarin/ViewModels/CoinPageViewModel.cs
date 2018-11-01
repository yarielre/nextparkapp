using GalaSoft.MvvmLight.Command;
using Inside.Xamarin.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Inside.Xamarin.Helpers;

namespace Inside.Xamarin.ViewModels
{
    public class CoinPageViewModel : BaseViewModel
    {
        public ObservableCollection<CoinSellItem> CoinsList { get; set; } = new ObservableCollection<CoinSellItem>();
        public double CoinPrice { get; set; }

        public ICommand BuyCoinsCommand => new RelayCommand<object>(BuyCoins);

        public CoinSellItem CoinSellOption1 { get; set; }
        public CoinSellItem CoinSellOption2 { get; set; }
        public CoinSellItem CoinSellOption3 { get; set; }
        public CoinSellItem CoinSellOption4 { get; set; }


        public CoinPageViewModel()
        {
            //TODO: Load from options list and coin price from backend!!!!
            CoinPrice = 0.5;

            CoinSellOption1 = new CoinSellItem
            {
                CoinsQuantity = 30,
                CoinBuyButtonCommand = this.BuyCoinsCommand
            };
            CoinSellOption1.CoinsQuantityPrice = CoinSellOption1.CoinsQuantity * this.CoinPrice;
            CoinSellOption1.CoinIcon = "coin.png";
            CoinSellOption1.CoinBuyButtonText = String.Format(Languages.CoinBuyNow, CoinSellOption1.CoinsQuantityPrice);
            CoinsList.Add(CoinSellOption1);

            CoinSellOption2 = new CoinSellItem
            {
                CoinsQuantity = 60,
                CoinBuyButtonCommand = this.BuyCoinsCommand
            };
            CoinSellOption2.CoinsQuantityPrice = CoinSellOption2.CoinsQuantity * this.CoinPrice;
            CoinSellOption2.CoinIcon = "coin.png";
            CoinSellOption2.CoinBuyButtonText = String.Format(Languages.CoinBuyNow, CoinSellOption1.CoinsQuantityPrice);
            CoinsList.Add(CoinSellOption2);

            CoinSellOption3 = new CoinSellItem
            {
                CoinsQuantity = 140,
                CoinBuyButtonCommand = this.BuyCoinsCommand
            };
            CoinSellOption3.CoinsQuantityPrice = CoinSellOption3.CoinsQuantity * this.CoinPrice;
            CoinSellOption3.CoinIcon = "coin.png";
            CoinSellOption3.CoinBuyButtonText = String.Format(Languages.CoinBuyNow, CoinSellOption1.CoinsQuantityPrice);
            CoinsList.Add(CoinSellOption3);

            CoinSellOption4 = new CoinSellItem
            {
                CoinsQuantity = 250,
                CoinBuyButtonCommand = this.BuyCoinsCommand
            };
            CoinSellOption4.CoinsQuantityPrice = CoinSellOption4.CoinsQuantity * this.CoinPrice;
            CoinSellOption4.CoinIcon = "coin.png";
            CoinSellOption4.CoinBuyButtonText = String.Format(Languages.CoinBuyNow, CoinSellOption1.CoinsQuantityPrice);
            CoinsList.Add(CoinSellOption4);

        }



        private async void BuyCoins(object coinsQuantity)
        {

            var currentUser = MainViewModel.GetInstance().CurrentUser;

            //TODO: Create in-app payment
            //TODO: On payment successfull increase user coins value
            coinsQuantity = Convert.ToDouble(coinsQuantity);

            var result = await DialogService.GetInstance().ShowDialogAlertOnMaster(string.Format(Languages.CoinBuyConfirmAlert, coinsQuantity), Languages.GeneralConfirm);

            if (result)
            {
                currentUser.Coins += Convert.ToInt32(coinsQuantity);
                var usermodel = await  DataService.GetInstance().AddCoins(new Domain.Models.UpdateUserCoinModel
                {
                    Coins = currentUser.Coins,
                    UserId = currentUser.Id
                });
                //Sync User info on backend
                MainViewModel.GetInstance().CurrentUser = currentUser;
                DialogService.GetInstance().ShowInfoAlertOnMaster(Languages.GeneralCongratulations, string.Format(Languages.CoinBuyCongratulationsAlert, coinsQuantity));
            }
        }
    }

    public class CoinSellItem
    {
        public int CoinsQuantity { get; set; }
        public double CoinsQuantityPrice { get; set; }
        public string CoinIcon { get; set; }
        public string CoinBuyButtonText { get; set; }
        public ICommand CoinBuyButtonCommand { get; set; }
    }
}
