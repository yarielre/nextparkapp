using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Enums.Enums
{
    public enum ErrorType
    {
        None,
        InternetConnectionError,
        Exception,
        EntityNull,
        ModelStateInvalid,
        EntityNotFound,
        ParkingNotVailable,
        ParkingNotOrderable,
        NotEnoughMoney,
        EventCantBeModified,
        ChangePasswordError,
        InAppPurchaseNotSupported,
        InAppPurchaseServiceConnectionError,
        InAppPurchaseServiceImposibleToPurchase,
        InAppPurchaseServiceSuccessPurchase,
        OrderCanNotBeFinished
    }
}
