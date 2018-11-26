using Microsoft.AspNetCore.Hosting;
using NextPark.Models;
using System;
using System.IO;

namespace NextPark.Services
{
    public class MediaService : IMediaService
    {
        public IHostingEnvironment _appEnvironment;
        public MediaService(IHostingEnvironment appEnvironment)
        {
            _appEnvironment = appEnvironment;
        }
        public bool UploadPhoto(MemoryStream stream, string folder, string name)
        {
            try
            {
                stream.Position = 0;
                var path = Path.Combine(folder, name);
                File.WriteAllBytes(path, stream.ToArray());
            }
            catch (Exception e)
            {
                throw e;
            }

            return true;
        }

        public void SaveParkingImage(ParkingModel model)
        {
            if (model.ImageBinary != null && model.ImageBinary.Length > 0)
            {
                var stream = new MemoryStream(model.ImageBinary);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var filePath = string.Format("/{0}/{1}", "Images", file);
                var folder = Path.Combine(_appEnvironment.WebRootPath, "Images");

                try
                {
                    var response = UploadPhoto(stream, folder, file);
                    if (response)
                    {
                        model.ImageUrl = filePath;
                        return;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }
}
