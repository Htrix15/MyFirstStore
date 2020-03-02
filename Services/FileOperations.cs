using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.Extensions.Logging;

namespace MyFirstStore.Services
{
    public class FileOperations
    {
        private readonly string pathRoot;
        private readonly ILogger<FileOperations> _logger;
        public FileOperations(IHostingEnvironment appEnvironment,
                              ILogger<FileOperations> logger)
        {
            pathRoot = appEnvironment.WebRootPath;
            _logger = logger;
        }
        public async Task<string> SavePictureAsync( string productsParentTypeName, string productsTypeName, string productName, IFormFile uploadedFile)
        {
            string subPath = $"/images/{productsParentTypeName}/{productsTypeName}/{productName.Replace(' ','_')}/";
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(pathRoot + subPath);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                using (var fileStream = new FileStream(pathRoot + subPath + uploadedFile.FileName, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Fail save pictures");
            }

            return subPath + uploadedFile.FileName;
        }
    }
}
