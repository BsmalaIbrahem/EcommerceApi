using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService) => _orderService = orderService;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            var language = HttpContext.Request.Headers["Accept-Language"]!.ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en"; // Default to English if no language is specified
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                var result = await _orderService.PlaceOrder(userId!, request, language);

                // لو النتيجة لينك (Stripe) بنرجعه، لو كلمة نجاح بنعرف الفرونت إنه كاش
                return Ok(new { url = result, isStripe = result.StartsWith("http") });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var language = HttpContext.Request.Headers["Accept-Language"]!.ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en"; // Default to English if no language is specified
            }
            var orders = await _orderService.GetUserOrders(userId!, language);
            return Ok(orders);
        }
    }
}
