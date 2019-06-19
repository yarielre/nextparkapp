using System.IO;

namespace NextPark.Services
{
    public interface IMediaService
    {
        string SaveImage(byte[] ImageBinary);
        bool UploadPhoto(MemoryStream stream, string folder, string name);
    }
}