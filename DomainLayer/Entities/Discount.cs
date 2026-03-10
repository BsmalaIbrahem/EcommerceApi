using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public enum DiscountTarget
    {
        Product = 1,
        Category = 2,
        Brand = 3,
        OrderTotal = 4, // خصم لو الأوردر عدى مبلغ معين
        CustomerLoyalty = 5 // خصم للناس اللي عملت أكتر من X أوردر
    }
    public class Discount : BaseEntity
    {
        public int Id { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsFlashSale { get; set; }
        public int Priority { get; set; } = 1;

        // جداول الربط (Navigation Properties)
        public ICollection<DiscountProduct> DiscountProducts { get; set; } = new List<DiscountProduct>();
        public ICollection<DiscountCategory> DiscountCategories { get; set; } = new List<DiscountCategory>();
        public ICollection<DiscountTranslation> DiscountTranslations { get; set; } = new List<DiscountTranslation>();
    }

    // جدول وسيط للمنتجات
    public class DiscountProduct
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public Discount Discount { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }

    // جدول وسيط للأقسام
    public class DiscountCategory
    {
        public int Id { get; set; }
        public int DiscountId { get; set; }
        public Discount Discount { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }

    public class DiscountTranslation : BaseTranslation
    {
        public int DiscountId { get; set; }
        public Discount Discount { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }

}
  
