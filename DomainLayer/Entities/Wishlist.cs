using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Wishlist : BaseEntity
    {
        public int Id { get; set; } // ممكن نستخدمه كـ surrogate key لو حبينا، بس مش ضروري لأنه عندنا composite key
        public string UserId { get; set; } = string.Empty; // ID المستخدم من الـ Auth
        public int ProductId { get; set; }

        // Navigation Properties
        public Product Product { get; set; } = null!;
    }
}
