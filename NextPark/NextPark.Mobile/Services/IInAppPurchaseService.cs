using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NextPark.Models;

namespace NextPark.Mobile.Services
{
    public interface IInAppPurchaseService
    {
        Task<ApiResponse> PurchaseCredit1();

        /// <summary>
        /// Purchases the credit 10
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        Task<ApiResponse> PurchaseCredit10();

        /// <summary>
        /// Purchases the credit 20
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        Task<ApiResponse> PurchaseCredit20();

        /// <summary>
        /// Purchases the credit 30
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        Task<ApiResponse> PurchaseCredit30();

        /// <summary>
        /// Purchases the credit 60
        /// </summary>
        /// <returns>ApiResponse. If success ApiResponse result is of type InAppBillingPurchase</returns>
        Task<ApiResponse> PurchaseCredit60();
    }
}
