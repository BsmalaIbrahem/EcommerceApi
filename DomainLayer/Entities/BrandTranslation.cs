using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class BrandTranslation : BaseTranslation
    {
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        public string Name { get; set; } = string.Empty;
    }
}
