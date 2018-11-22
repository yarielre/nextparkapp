using System.IO;
using NextPark.Models;

namespace NextPark.Services
{
    public interface IMediaService
    {
        void SaveParkingImage(ParkingModel model);
        bool UploadPhoto(MemoryStream stream, string folder, string name);
    }
}