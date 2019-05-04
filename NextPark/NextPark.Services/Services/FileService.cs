using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using NextPark.Domain.Entities;
using NextPark.Models;

namespace NextPark.Services.Services
{
    public class FileService : IFileService
    {
        private const string HostedFilesFolderName = "HostedServiceFiles";

        public string HostedFolder { get; set; }

        public FileService(IHostingEnvironment appEnvironment)
        {
            this.HostedFolder = Path.Combine(appEnvironment.WebRootPath, HostedFilesFolderName);
        }
        public void CreateOrderFileHosted(Order orderModel)
        {
            var endTime = orderModel.EndDate.Ticks;
            
            using (var text = new StreamWriter($"{HostedFolder}\\{endTime}_{orderModel.Id}.fl"))
            {
                text.Write(DateTime.Now.ToString("g"));
            }
        }

        public void DeleteOrderFileHosted(int orderId)
        {
            var files = GetHostedFiles();
            foreach (var file in files)
            {
                var extension = file.Extension;
                var cutx = file.Name.Split('_');
                var id = int.Parse(cutx[1].Replace(extension, "")); //order id
                if (orderId == id)
                {
                    file.Delete();
                }
            }
        }

        public FileInfo[] GetHostedFiles()
        {
            var hostedServicesFilesFolder = new DirectoryInfo(HostedFolder);
            return hostedServicesFilesFolder.GetFiles();
        }
    }
}
