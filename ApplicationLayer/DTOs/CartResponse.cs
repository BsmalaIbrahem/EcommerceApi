using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class CartItemResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } // السعر الأصلي
        public decimal FinalPrice { get; set; } // السعر بعد خصم الـ Product/Category
        public decimal TotalLinePrice => FinalPrice * Quantity;
    }

    public class CartResponse
    {
        public IEnumerable<CartItemResponse> Items { get; set; } = new List<CartItemResponse>();
        public decimal TotalCartPrice => Items.Sum(x => x.TotalLinePrice);
    }
}
