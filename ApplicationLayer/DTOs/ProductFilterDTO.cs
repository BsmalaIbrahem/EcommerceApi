using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class ProductFilterDTO
    {
        public string? SearchText { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public bool? IsOrganic { get; set; }
        public bool? SortedByPrice { get; set; }
        public bool? sortedByDesc { get; set; }
    }
}
