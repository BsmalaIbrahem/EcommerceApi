using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public enum OrderStatus { Pending, Processing, Shipped, Delivered, Cancelled, Refunded }
    public enum PaymentMethod { Cash, Visa }

    public class Order : BaseEntity
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentMethod PaymentMethod { get; set; }
        public bool IsPaid { get; set; } = false;

        // بيانات الشحن (ممكن ناخدها من الـ Address Module لاحقاً)
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        // لربط Stripe
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem : BaseEntity
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; } // السعر وقت الشراء
    }
}
