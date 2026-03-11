using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Cart : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }

    public class CartItem : BaseEntity
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }
    }
}
