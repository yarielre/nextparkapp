using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using NextPark.Models;

namespace NextPark.Services.Services
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _appEnvironment;

        public FileService(IHostingEnvironment appEnvironment)
        {
            this._appEnvironment = appEnvironment;
        }
        public void CreateFile(OrderModel orderModel)
        {
            var endTime = orderModel.EndDate.Ticks;
            var folder = Path.Combine(this._appEnvironment.WebRootPath, "HostedServiceFiles");
            using (var text = new StreamWriter($"{folder}\\{endTime}_{orderModel.Id}.fl"))
            {
                text.Write(DateTime.Now.ToString("g"));
            }
        }
    }
}
