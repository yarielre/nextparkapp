using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Inside.Xamarin.Services
{
    public class InAppBillingService
    {
        public async Task<bool> MakePurchase()
        {
            if (!CrossInAppBilling.IsSupported)
                return false;

            var billing = CrossInAppBilling.Current;

            try
            {

                var connected = await billing.ConnectAsync(ItemType.InAppPurchase);
                if (!connected)
                    return false;

                //make additional billing calls

                return true;
            }
            catch {
                return false;
            }
            finally
            {
                await billing.DisconnectAsync();
            }
        }
    }
}
