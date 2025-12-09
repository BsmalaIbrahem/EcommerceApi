using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class EditBrandRequest
    {
        public IFormFile? Image { get; set; }
        //[LanguageCode]
        public string TranslationsJson { get; set; } = null!;
    }
}
