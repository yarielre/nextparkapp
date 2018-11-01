using AutoMapper;
using Inside.Domain.Entities;
using Inside.WebApi.ViewModels;

namespace Inside.WebApi.MapperTools
{
    public class MarkerUrlResolver : IValueResolver<Parking, ParkingViewModel, string>
    {
        public string Resolve(Parking source, ParkingViewModel destination, string destMember,
            ResolutionContext context)
        {
            if (source.IsRented)
                return "http://maps.google.com/mapfiles/ms/icons/purple-dot.png";
            if (source.ParkingCategory.Category == "Bussines")
                return "http://maps.google.com/mapfiles/ms/icons/green-dot.png";
            return "http://maps.google.com/mapfiles/ms/icons/red-dot.png";
        }
    }
}