using AutoMapper;
using Inside.Domain.Entities;
using Inside.Domain.Models;

namespace NextPark.MapperTools
{
    public class ImageUrlResolver:IValueResolver<ParkingModel,Parking,string>
    {
        public string Resolve(ParkingModel source, Parking destination, string destMember, ResolutionContext context)
        {
            string[] array = source.ImageUrl.Split('/');
            return array[array.Length - 1];

        }
    }
}
