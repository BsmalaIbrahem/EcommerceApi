using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }

    public class CategoryWithTranslatedResponse
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public List<CategoryTranslationDto> Translations { get; set; } = null!;
    }
}
