using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inside.Xamarin.Helpers
{
    public class FilesHelper
    {
        public static byte[] ReadFully(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
