using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class CreateProductRequest
    {
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public bool IsOrganic { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public IFormFile? Image { get; set; }
        public string TranslationsJson { get; set; } = null!; // JSON string of List<ProductTranslationDto>
    }

    public class ProductTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
    }
}
