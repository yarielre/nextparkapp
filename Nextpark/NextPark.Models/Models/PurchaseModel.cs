namespace NextPark.Models
{
    public class PurchaseModel : BaseModel
    {
        public int UserId { get; set; }
        public double Cash { get; set; }
        public double NewUserBalance { get; set; }
        public double NewUserProfit { get; set; }
    }
}