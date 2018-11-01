using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
