using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public decimal Price { get; set; } // السعر الأساسي فقط
        public int StockQuantity { get; set; }
        public string? ImagePath { get; set; }
        public bool IsOrganic { get; set; }

        // التنبيه للمخزون (مستوحى من صورك: available only 23)
        public int LowStockThreshold { get; set; } = 10;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public ICollection<ProductTranslation> ProductTranslations { get; set; } = new List<ProductTranslation>();
    }

    public class ProductTranslation : BaseTranslation
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty; // مثلاً: 64 fl oz أو 1kg
    }
}
