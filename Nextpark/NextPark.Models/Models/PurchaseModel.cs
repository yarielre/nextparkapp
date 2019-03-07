using NextPark.Enums;
using NextPark.Enums.Enums;
using System;
using System.Collections.Generic;

namespace NextPark.Models
{
    public class PurchaseModel : BaseModel
    {
        public int UserId { get; set; }
        public double CashToAdd { get; set; }
        public double NewUserBalance { get; set; }
    }
}