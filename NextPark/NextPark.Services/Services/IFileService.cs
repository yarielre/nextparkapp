using NextPark.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace NextPark.Services.Services
{
   public interface IFileService
    {
        void CreateFile(OrderModel orderModel);
    }
}
