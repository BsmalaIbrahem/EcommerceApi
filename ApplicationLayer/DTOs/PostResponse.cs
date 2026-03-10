using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class CreatePostRequest
    {
        public string AuthorName { get; set; } = "Admin";
        public IFormFile? Image { get; set; }
        public string TranslationsJson { get; set; } = null!; // List<PostTranslationDto>
    }

    public class PostTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class PostWithTranslatedResponse
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public string AuthorName { get; set; } = "Admin";
        public List<PostTranslationDto> Translations { get; set; } = null!;
    }

    public class PostResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = "Admin";
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
