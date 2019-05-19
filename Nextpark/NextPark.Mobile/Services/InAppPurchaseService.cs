using System;
using System.Threading.Tasks;
using NextPark.Enums.Enums;
using NextPark.Models;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;

namespace NextPark.Mobile.Services
{

    public class InAppPurchaseService : IInAppPurchaseService
    {
        private readonly string NextParkCredit1 = "nextpark_credit_1";
        private readonly string NextParkCredit10 = "nextpark_credit_10";
        private readonly string NextParkCredit20 = "nextpark_credit_20";
        private readonly string NextParkCredit30 = "nextpark_credit_30";
        private readonly string NextParkCredit60 = "nextpark_credit_60";


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

        /// <summary>
        /// Purchases the credit 1
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit1()
        {
            return await this.MakePurchase(NextParkCredit1);
        }
        /// <summary>
        /// Purchases the credit 10
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit10()
        {
            return await MakePurchase(NextParkCredit10);
        }
        /// <summary>
        /// Purchases the credit 20
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit20()
        {
            return await MakePurchase(NextParkCredit20);
        }
        /// <summary>
        /// Purchases the credit 30
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit30()
        {
            return await MakePurchase(NextParkCredit30);
        }
        /// <summary>
        /// Purchases the credit 50
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit60()
        {
            return await MakePurchase(NextParkCredit60);
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

                var purchase = await Billing.PurchaseAsync(productId, ItemType.InAppPurchase, "nextpark") ;

                if (purchase == null)
                {
                    result.IsSuccess = false;
                    result.Message = Enum.GetName(typeof(ErrorType), ErrorType.InAppPurchaseServiceImposibleToPurchase);
                    result.ErrorType = ErrorType.InAppPurchaseServiceImposibleToPurchase;
                    return result;
                }
                else 
                {
                    if (purchase.State == PurchaseState.Purchased)
                    {   
                        // Item pusrchaesd
                        if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.Android)
                        {
                            // Only on Android devices try to consume the item
                            var consumedItem = await CrossInAppBilling.Current.ConsumePurchaseAsync(purchase.ProductId, purchase.PurchaseToken);

                            if (consumedItem != null)
                            {
                                //Consumed!!
                            }
                        }
                    }

                    //Purchased, save this information
                    var id = purchase.Id;
                    var token = purchase.PurchaseToken;
                    var state = purchase.State;

                    result.IsSuccess = true;
                    result.Message = Enum.GetName(typeof(ErrorType), ErrorType.InAppPurchaseServiceSuccessPurchase);
                    result.ErrorType = ErrorType.InAppPurchaseServiceSuccessPurchase;
                    result.Result = purchase;
                   
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
