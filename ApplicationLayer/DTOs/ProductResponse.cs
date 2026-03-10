using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public decimal Price { get; set; } // السعر الأصلي
        public decimal FinalPrice { get; set; } // السعر بعد أكبر خصم
        public string? ImagePath { get; set; }
        public bool IsOrganic { get; set; }
        public int StockQuantity { get; set; }
        public bool IsFlashSale { get; set; } // عشان الـ Countdown
        public decimal? DiscountPercentage { get; set; } // نسبة الخصم لو كان خصم بالنسبة المئوية
        public DateTime? OfferEndDate { get; set; } // نهاية الخصم لو كان خصم بالنسبة المئوية
    }

    public class ProductWithTranslatedResponse
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImagePath { get; set; }
        public int CategoryId { get; set; }
        public int? BrandId { get; set; }
        public bool IsOrganic { get; set; }
        public List<ProductTranslationDto> Translations { get; set; } = new();
    }
}
