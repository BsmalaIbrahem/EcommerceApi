using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Coupon : BaseEntity
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // الكود اللي بيدخله العميل
        public int DiscountId { get; set; } // مرتبط بجدول الخصومات اللي فوق
        public Discount Discount { get; set; } = null!;
        public int UsageLimit { get; set; } // الكود يستخدم كام مرة إجمالاً؟
        public int TimesUsed { get; set; } // استخدم كام مرة فعلاً؟
    }
}
