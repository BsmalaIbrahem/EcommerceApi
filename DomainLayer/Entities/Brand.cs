using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Brand : BaseEntity
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public ICollection<BrandTranslation> BrandTranslations { get; set; } = new List<BrandTranslation>();
    }
}
