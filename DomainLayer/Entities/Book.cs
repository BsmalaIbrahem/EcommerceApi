using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Book : BaseEntity
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime? PublishedDate { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public double Price { get; set; }
        public string ImgPath { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}
