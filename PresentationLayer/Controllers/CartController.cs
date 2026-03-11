using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace PresentationLayer.Controllers
{
    
    [Authorize] // كل العمليات هنا محتاجة Token
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // 1. جلب محتويات السلة للمستخدم الحالي باللغة المطلوبة
        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var language = HttpContext.Request.Headers["Accept-Language"]!.ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en"; // Default to English if no language is specified
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetUserCart(userId!, language);
            return Ok(ApiResponse<CartResponse>.SuccessResponse(200, "Home page data retrieved successfully", cart));
        }

        // 2. إضافة منتج للسلة أو تحديث الكمية (القيمة المطلقة)
        [HttpPost("add-or-update")]
        public async Task<IActionResult> AddOrUpdate([FromBody] CartItemRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddOrUpdateItem(userId!, request.ProductId, request.Quantity);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Cart updated successfully", null));
        }

        // 3. دمج سلة الزائر (Guest Cart) مع سلة المستخدم المسجل
        // بتنادي عليها مرة واحدة فوراً بعد الـ Login
        [HttpPost("merge")]
        public async Task<IActionResult> Merge([FromBody] List<CartItemRequest> guestItems)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.MergeGuestCartIntoUserCart(userId!, guestItems);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Cart merged successfully", null));
        }

        // 4. زيادة الكمية بمقدار 1 (لزرار الـ + في الصورة)
        [HttpPatch("increment/{productId}")]
        public async Task<IActionResult> Increment(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.UpdateQuantity(userId!, productId, 1);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Quantity increased!", null));
        }

        // 5. تقليل الكمية بمقدار 1 (لزرار الـ - في الصورة)
        [HttpPatch("decrement/{productId}")]
        public async Task<IActionResult> Decrement(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.UpdateQuantity(userId!, productId, -1);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Quantity decreased", null));
        }

        // 6. حذف منتج معين تماماً من السلة
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> Remove(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.RemoveItem(userId!, productId);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Item removed from cart", null));
        }


        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var count = await _cartService.GetCartItemsCount(userId!);
            return Ok(ApiResponse<int>.SuccessResponse(200, "Home page data retrieved successfully", count));
        }

    }
}
