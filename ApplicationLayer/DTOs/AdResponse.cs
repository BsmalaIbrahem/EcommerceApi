using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class AdTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string MainTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
    }

    public class CreateAdRequest
    {
        public IFormFile Image { get; set; } = null!;
        public AdTargetType TargetType { get; set; }
        public int? TargetId { get; set; }
        public string? ExternalUrl { get; set; }
        public int Priority { get; set; }
        public string TranslationsJson { get; set; } = null!; // List<AdTranslationDto>
    }

    public class AdResponse
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public string SmallTitle { get; set; } = string.Empty;
        public string MainTitle { get; set; } = string.Empty;
        public string SubTitle { get; set; } = string.Empty;
        public AdTargetType TargetType { get; set; }
        public int? TargetId { get; set; }
        public string? TargetUrl { get; set; } // اللينك النهائي المحسوب
    }
}
