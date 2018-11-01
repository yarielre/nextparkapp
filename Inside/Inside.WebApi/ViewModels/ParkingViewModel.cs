using Inside.Domain.Entities;

namespace Inside.WebApi.ViewModels
{
    public class ParkingViewModel : BaseViewModel
    {
        public int ParkingCategoryId { get; set; }
        public ParkingCategoryViewModel ParkingCategory { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public bool IsRented { get; set; }
        public string Status { get; set; }
        public Order RentInfo { get; set; }
        public int UserId { get; set; }
        public string ImageUrl { get; set; }
        public ParkingTypeViewModel ParkingType { get; set; }
        public EventViewModel ParkingEvent { get; set; }
        public int ParkingEventId { get; set; }
        public int ParkingTypeId { get; set; }
    }
}