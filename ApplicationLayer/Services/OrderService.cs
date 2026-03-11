using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Order> _orderRepo;

        private readonly ICartService _cartService;

        public OrderService(ICartService cartService, IRepository<Order> _orderRepo)
        {
            _cartService = cartService;
            this._orderRepo = _orderRepo;
        }

        public async Task<string> PlaceOrder(string userId, CheckoutRequest request, string language)
        {
            // 1. جلب السلة الحالية (باستخدام الخدمة اللي عملناها)
            var cart = await _cartService.GetUserCart(userId, language);
            if (!cart.Items.Any()) throw new Exception("Cart is empty");

            // 2. إنشاء كائن الطلب
            var order = new Order
            {
                UserId = userId,
                TotalPrice = cart.TotalCartPrice,
                Status = OrderStatus.Pending,
                PaymentMethod = request.PaymentMethod,
                Address = request.Address,
                City = request.City,
                Phone = request.Phone,
                OrderItems = cart.Items.Select(item => new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.FinalPrice // السعر وقت الشراء
                }).ToList()
            };

            // 3. حفظ الطلب في قاعدة البيانات (Repo)
            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();

            // 4. لو الدفع كاش (Cash On Delivery)
            if (request.PaymentMethod == PaymentMethod.Cash)
            {
                await _cartService.ClearCart(userId);
                return "CashOrderSuccess";
            }

            // 5. لو الدفع فيزا (Stripe Session)
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = cart.Items.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.FinalPrice * 100), // السعر بالسنت
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.ProductName
                        }
                    },
                    Quantity = item.Quantity
                }).ToList(),
                Mode = "payment",
                // الروابط دي بتبعتيها من الـ Frontend في الـ Request
                SuccessUrl = request.SuccessUrl + "?orderId=" + order.Id,
                CancelUrl = request.CancelUrl + "?orderId=" + order.Id,
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            // تحديث الطلب بالـ SessionId للـ Tracking
            order.SessionId = session.Id;
            await _orderRepo.SaveChangesAsync();

            return session.Url; // نرجع لينك بوابة الدفع
        }

        public async Task<IEnumerable<OrderResponse>> GetUserOrders(string userId, string lang)
        {
            var orders = await _orderRepo.GetAllAsync(
                filters: [o => o.UserId == userId && (o.IsPaid || o.PaymentMethod == PaymentMethod.Cash)],
                includeChain: q => q.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.ProductTranslations).ThenInclude(t => t.Language)
            );

            return MapOrdersToResponse(orders, lang);
        }

        public async Task<IEnumerable<OrderResponse>> GetAdminOrders(OrderStatus? status, bool? isPaid, string? userId, string lang)
        {
            var filters = new List<Expression<Func<Order, bool>>>();

            if (status.HasValue) filters.Add(o => o.Status == status);
            if (isPaid.HasValue) filters.Add(o => o.IsPaid == isPaid);
            if (!string.IsNullOrEmpty(userId)) filters.Add(o => o.UserId == userId);

            var orders = await _orderRepo.GetAllAsync(
                filters: filters.ToArray(),
                includeChain: q => q.Include(o => o.OrderItems).ThenInclude(oi => oi.Product).ThenInclude(p => p.ProductTranslations).ThenInclude(t => t.Language),
                orderBy: q => q.OrderByDescending(o => o.CreatedAt)
            );

            return MapOrdersToResponse(orders, lang);
        }

        // ميثود مساعدة للتحويل (Mapping)
        private IEnumerable<OrderResponse> MapOrdersToResponse(IEnumerable<Order> orders, string lang)
        {
            return orders.Select(o => new OrderResponse
            {
                OrderId = o.Id,
                TotalPrice = o.TotalPrice,
                Status = o.Status.ToString(),
                IsPaid = o.IsPaid,
                CreatedAt = o.CreatedAt,
                PaymentMethod = o.PaymentMethod.ToString(),
                Items = o.OrderItems.Select(oi => new OrderItemResponse
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product.ProductTranslations.Where(p => p.Language.Code == lang).FirstOrDefault()?.Name ?? "", // تأكدي من التعامل مع اللغة هنا لو محتاجة
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            });
        }

    }
}
