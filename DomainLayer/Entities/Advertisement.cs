using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public enum AdTargetType
    {
        Category = 1,
        Product = 2,
        AllOffers = 3,
        ExternalLink = 4
    }

    public class Advertisement : BaseEntity
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public AdTargetType TargetType { get; set; }
        public int? TargetId { get; set; } // هيكون ID المنتج أو القسم
        public string? ExternalUrl { get; set; } // لو اللينك خارجي
        public int Priority { get; set; } = 1;
        public bool IsActive { get; set; } = true;

        public ICollection<AdTranslation> AdTranslations { get; set; } = new List<AdTranslation>();
    }

    public class AdTranslation : BaseTranslation
    {
        public int Id { get; set; }
        public int AdId { get; set; }
        [ForeignKey("AdId")]
        public Advertisement Advertisement { get; set; } = null!;
        public string SmallTitle { get; set; } = string.Empty; // "Only This Week"
        public string MainTitle { get; set; } = string.Empty;  // "We provide you..."
        public string SubTitle { get; set; } = string.Empty;   // "A family place..."
    }
}
