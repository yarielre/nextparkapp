using System.Threading.Tasks;
using Plugin.InAppBilling;
using Plugin.InAppBilling.Abstractions;

namespace NextPark.Mobile.Services
{
    public class InAppPurchaseService
    {

        public string ProductId = "parking1chf";

        public IInAppBilling Billing { get; set; }

        public InAppPurchaseService()
        {
            Billing = CrossInAppBilling.Current;

        }

        public async Task<bool> MakePurchase()
        {
            if (!CrossInAppBilling.IsSupported)
                return false;

            Billing.InTestingMode = true;

            try
            {

                var connected = await Billing.ConnectAsync(ItemType.InAppPurchase);
                if (!connected)
                    return false;

                //TODO: Fix payload
                var purchase = await Billing.PurchaseAsync(ProductId, ItemType.InAppPurchase, "payload");

               

            }
            finally
            {
                await Billing.DisconnectAsync();
            }

            return true;
        }
    }
}
