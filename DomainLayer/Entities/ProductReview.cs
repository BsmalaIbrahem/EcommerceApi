using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class ProductReview : BaseEntity
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; } // من 1 لـ 5
        public string? Comment { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
