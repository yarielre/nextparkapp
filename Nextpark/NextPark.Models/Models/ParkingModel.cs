using NextPark.Domain.Entities;
using NextPark.Enums.Enums;

namespace NextPark.Models
{
    public class ParkingModel : BaseModel
    {
        public string ImageUrl { get; set; }
        public byte[] ImageBinary { get; set; }

        public string Address { get; set; }
        public int Cap { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string CarPlate { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public int UserId { get; set; }

        public double PriceMin { get; set; }
        public double PriceMax { get; set; }

        public ParkingStatus Status { get; set; }
    }

    public static partial class ModelExtensions
    {
        public static Parking ToParking(this ParkingModel model)
        {

            return new Parking
            {
                Id = model.Id,
                Address = model.Address,
                Cap = model.Cap,
                CarPlate = model.CarPlate,
                City = model.City,
                ImageUrl = model.ImageUrl,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                PriceMax = model.PriceMax,
                PriceMin = model.PriceMin,
                State = model.State,
                Status = model.Status,
                UserId = model.UserId
            };
        }

        public static ParkingModel ToParkingModel(this Parking entity)
        {

            return new ParkingModel
            {
                Id = entity.Id,
                Address = entity.Address,
                Cap = entity.Cap,
                CarPlate = entity.CarPlate,
                City = entity.City,
                ImageUrl = entity.ImageUrl,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                PriceMax = entity.PriceMax,
                PriceMin = entity.PriceMin,
                State = entity.State,
                Status = entity.Status,
                UserId = entity.UserId
            };
        }
    }
}

