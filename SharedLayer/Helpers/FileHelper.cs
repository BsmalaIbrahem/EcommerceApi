using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLayer.Helpers
{
    public static class FileHelper
    {
        public static async Task<(string FileName, string RelativePath)> UploadAndSaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is null or empty.");

            // اسم فريد للملف
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // المسار الكامل على السيرفر
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var filePath = Path.Combine(folderPath, fileName);

            // رفع الملف
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // المسار النسبي بالنسبة لـ wwwroot
            var relativePath = Path.Combine(folderName, fileName).Replace("\\", "/");

            return (fileName, relativePath);
        }
            

        public static void DeleteFile(string relativePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

    }
}
