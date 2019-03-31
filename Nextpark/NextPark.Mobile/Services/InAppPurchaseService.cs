using System;
using System.Threading.Tasks;
using NextPark.Enums.Enums;
using NextPark.Models;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;

namespace NextPark.Mobile.Services
{

    public class InAppPurchaseService
    {
        private readonly string ProductCredit1 = "parking_credit_chf1";
        private readonly string ProductCredit10 = "parking_credit_chf10";

        public bool InTestMode
        {
            get
            {
                if (Billing == null) return false;
                return Billing.InTestingMode;
            }
            set
            {
                if (Billing != null) Billing.InTestingMode = value;
            }
        }

        public IInAppBilling Billing { get; set; }

        public InAppPurchaseService()
        {
            Billing = CrossInAppBilling.Current;
            Billing.InTestingMode = false;
        }

        public async Task<ApiResponse> PurchaseCreadit1()
        {
            
            return await this.MakePurchase(ProductCredit1);
        }

        public async Task<ApiResponse> PurchaseCreadit10()
        {
            return await MakePurchase(ProductCredit10);
        }

        private async Task<ApiResponse> MakePurchase(string productId)
        {
            var result = new ApiResponse();

            if (!CrossInAppBilling.IsSupported)
            {
                result.IsSuccess = false;
                result.Message = Enum.GetName(typeof(ErrorType), ErrorType.InAppPurchaseNotSupported);
                result.ErrorType = ErrorType.InAppPurchaseNotSupported;
                return result;
            }
            
            try
            {
                var connected = await Billing.ConnectAsync(ItemType.InAppPurchase);
                if (!connected)
                {
                    result.IsSuccess = false;
                    result.Message = Enum.GetName(typeof(ErrorType), ErrorType.InAppPurchaseServiceConnectionError);
                    result.ErrorType = ErrorType.InAppPurchaseServiceConnectionError;
                    return result;
                }
                    
                //var info = await Billing.GetProductInfoAsync(ItemType.InAppPurchase, ProductCredit1, ProductCredit10);

                //TODO: Fix payload
                var purchase = await Billing.PurchaseAsync(productId, ItemType.InAppPurchase, "apppayload") ;

                if (purchase == null)
                {
                    //Not purchased, alert the user
                    result.IsSuccess = false;
                    result.Message = Enum.GetName(typeof(ErrorType), ErrorType.InAppPurchaseServiceImposibleToPurchase);
                    result.ErrorType = ErrorType.InAppPurchaseServiceImposibleToPurchase;
                    return result;
                }
                else
                {
                    //Purchased, save this information
                    var id = purchase.Id;
                    var token = purchase.PurchaseToken;
                    var state = purchase.State;

                    result.IsSuccess = true;
                    result.Message = Enum.GetName(typeof(ErrorType), ErrorType.InAppPurchaseServiceSuccessPurchase);
                    result.ErrorType = ErrorType.InAppPurchaseServiceSuccessPurchase;
                   
                }
            }
            catch(Exception ex) {
                result.IsSuccess = false;
                result.Message = ex.Message;
                result.ErrorType = ErrorType.Exception;
                return result;
            }
            finally
            {
                await Billing.DisconnectAsync();
            }

            return result;
        }


    }
}
