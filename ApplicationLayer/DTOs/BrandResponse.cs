using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class BrandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
    }

    public class BrandWithTranslatedResponse
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public List<BrandTranslationDto> Translations { get; set; } = null!;
    }
}
