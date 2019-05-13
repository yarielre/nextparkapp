using NextPark.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NextPark.Domain.Entities;

namespace NextPark.Services.Services
{
   public interface IFileService
    {
        void CreateOrderFileHosted(Order order);
        FileInfo[] GetHostedFiles();
        void DeleteOrderFileHosted(int orderId);
    }
}
