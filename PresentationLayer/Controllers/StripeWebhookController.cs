using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.BillingPortal;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StripeWebhookController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IRepository<Order> _orderRepo;
        private readonly IConfiguration _configuration;

        private readonly IRepository<DomainLayer.Entities.Product> _productRepo; // لازم تجيبيه عشان تقللي الكميات من المخزن

        public StripeWebhookController(IOrderService orderService, ICartService cartService, IRepository<Order> orderRepo, IRepository<DomainLayer.Entities.Product> productRepo, IConfiguration configuration)
        {
            _orderService = orderService;
            _cartService = cartService;
            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                string _webhookSecret = _configuration["Stripe:WebhookSecret"];
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _webhookSecret
                );

                if (stripeEvent.Type == EventTypes.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    // بنجيب الطلب ومعاه الـ Items عشان نعرف نقلل كمياتها
                    var order = await _orderRepo.GetOneAsync(
                        filters: [o => o.SessionId == session.Id],
                        includeChain: q => q.Include(o => o.OrderItems)
                    );

                    if (order != null)
                    {
                        // 1. تحديث حالة الطلب
                        order.IsPaid = true;
                        order.Status = OrderStatus.Processing;
                        order.PaymentIntentId = session?.Id;

                        // 2. تقليل كمية المنتجات من المخزن
                        foreach (var item in order.OrderItems)
                        {
                            var product = await _productRepo.GetOneAsync([x=> x.Id == item.ProductId]);
                            if (product != null)
                            {
                                // بننقص الكمية المتباعة من الـ Stock
                                product.StockQuantity -= item.Quantity;

                                // تأكدي إن الحقل ده اسمه StockQuantity أو حسب ما سميتيه في الـ Product Entity
                                 _productRepo.UpdateAsync(product);
                            }
                        }

                        await _orderRepo.SaveChangesAsync();
                        await _productRepo.SaveChangesAsync();

                        // 3. مسح السلة
                        await _cartService.ClearCart(order.UserId);
                    }
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }
    }
}
