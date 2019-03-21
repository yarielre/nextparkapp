using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using NextPark.Services.Services.Interfaces;

namespace NextPark.Services.Services.Services
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

        public string SaveImage(byte[] ImageBinary)
        {
            var imageUrl = string.Empty;

            if (ImageBinary != null && ImageBinary.Length > 0)
            {
                var stream = new MemoryStream(ImageBinary);
                var guid = Guid.NewGuid().ToString();
                var file = string.Format("{0}.jpg", guid);
                var filePath = string.Format("/{0}/{1}", "images", file);
                var folder = Path.Combine(_appEnvironment.WebRootPath, "images");
             
                try
                {
                    var response = UploadPhoto(stream, folder, file);
                    if (response)
                    {
                        imageUrl =  filePath;
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return imageUrl;
        }
    }
}
