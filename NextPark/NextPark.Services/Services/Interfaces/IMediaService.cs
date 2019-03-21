using System.IO;

namespace NextPark.Services.Services.Interfaces
{
    public interface IMediaService
    {
        string SaveImage(byte[] ImageBinary);
        bool UploadPhoto(MemoryStream stream, string folder, string name);
    }
}