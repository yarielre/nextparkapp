namespace NextPark.Models
{
    public class ParkingCategoryModel : BaseModel
    {
        public string Category { get; set; }
        public double HourPrice { get; set; }
        public double MonthPrice { get; set; }
    }
}