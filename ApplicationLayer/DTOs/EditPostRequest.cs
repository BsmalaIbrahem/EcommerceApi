using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class EditPostRequest
    {
        // اسم الكاتب لو حابة تغيريه (مثلاً من Admin لاسم شخص محدد)
        public string AuthorName { get; set; } = "Admin";

        // الصورة اختيارية، لو مبعتش صورة السيرفر هيحتفظ بالقديمة
        public IFormFile? Image { get; set; }

        // الترجمات بتتبعت كـ JSON String عشان الـ form-data
        // [ { "LanguageCode": "ar", "Title": "...", "Content": "..." }, ... ]
        public string? TranslationsJson { get; set; }
    }
}
