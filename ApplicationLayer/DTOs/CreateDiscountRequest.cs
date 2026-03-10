using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class CreateDiscountRequest
    {
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsFlashSale { get; set; }
        public string TranslationsJson { get; set; } = null!;

        // اللستة الجديدة للأهداف
        public List<int>? ProductIds { get; set; }
        public List<int>? CategoryIds { get; set; }
    }

    public class DiscountTranslationDto
    {
        public string LanguageCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;       // مثلاً: "عروض الجمعة البيضاء"
        public string? Description { get; set; }               // مثلاً: "خصم يصل إلى 50% على جميع المنتجات"
    }

}
