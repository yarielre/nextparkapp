using System;
using System.IO;

namespace Inside.Web.Helpers
{
    public class FilesHelper
    {
        public static bool UploadPhoto(MemoryStream stream, string folder, string name)
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
    }
}
