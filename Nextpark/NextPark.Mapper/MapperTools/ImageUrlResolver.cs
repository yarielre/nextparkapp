using AutoMapper;
using NextPark.Domain.Entities;
using NextPark.Models;

namespace NextPark.MapperTools
{
    public class ImageUrlResolver:IValueResolver<ParkingModel,Parking,string>
    {
        public string Resolve(ParkingModel source, Parking destination, string destMember, ResolutionContext context)
        {
            string[] array = source.ImageUrl.Split('/');
            return array[array.Length - 1];

        }
        public string Resolve(UserModel source, ApplicationUser destination, string destMember, ResolutionContext context)
        {
            string[] array = source.ImageUrl.Split('/');
            return array[array.Length - 1];

        }
    }
}
