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
        private readonly string ProductCredit20 = "parking_credit_chf20";
        private readonly string ProductCredit30 = "parking_credit_chf30";
        private readonly string ProductCredit50 = "parking_credit_chf50";
        private readonly string NextParkCredit1 = "nextpark_credit_1";


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
            return await MakePurchase(ProductCredit10);
        }
        /// <summary>
        /// Purchases the credit 20
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit20()
        {
            return await MakePurchase(ProductCredit20);
        }
        /// <summary>
        /// Purchases the credit 30
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit30()
        {
            return await MakePurchase(ProductCredit30);
        }
        /// <summary>
        /// Purchases the credit 50
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        public async Task<ApiResponse> PurchaseCredit50()
        {
            return await MakePurchase(ProductCredit50);
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

                InAppBillingPurchase purchase = await Billing.PurchaseAsync(productId, ItemType.InAppPurchase, "apppayload") ;

                if (purchase == null)
                {
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
