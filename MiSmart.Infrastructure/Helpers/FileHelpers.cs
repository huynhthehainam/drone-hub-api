using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiSmart.Infrastructure.Constants;
using Microsoft.AspNetCore.Http;
using System;

namespace MiSmart.Infrastructure.Helpers
{
    public static class FileHelpers
    {
        public static String SaveFile(IFormFile file, String[] paths)
        {
            if (file is Object)
            {
                var extension = file.FileName.Split(".", System.StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                var aa = Path.GetRandomFileName();
                var fileName = String.Join(".", new String[] { aa, extension });
                var filePaths = new List<String>() { FolderPaths.StaticFilePath };
                filePaths.AddRange(paths);
                var tempPath = Path.Combine(filePaths.ToArray()).Replace("\\", "/");
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                filePaths.Add(fileName);
                var filePath = Path.Combine(filePaths.ToArray()).Replace("\\", "/");
                using (Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var urls = new List<String>();
                urls.AddRange(paths);
                urls.Add(fileName);
                var url = Path.Combine(urls.ToArray()).Replace("\\", "/");
                return url;
            }
            return null;
        }
        public static Boolean RemoveFileByUrl(String url)
        {
            var filePath = $"{FolderPaths.StaticFilePath}/{url}";
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}