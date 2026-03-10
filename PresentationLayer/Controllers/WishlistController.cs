using ApplicationLayer.Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PresentationLayer.Controllers
{
    [Authorize] // لازم يكون عامل تسجيل دخول
    [ApiController]
    [Route("api/[controller]")]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;
        public WishlistController(IWishlistService wishlistService) => _wishlistService = wishlistService;

        // إضافة أو حذف منتج من القائمة
        [HttpPost("toggle/{productId}")]
        public async Task<IActionResult> Toggle(int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // جلب الـ ID من التوكن
            await _wishlistService.ToggleWishlist(userId!, productId);
            return Ok(new { message = "Wishlist updated" });
        }

        // جلب القائمة بالكامل للغة معينة
        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var language = HttpContext.Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en";
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _wishlistService.GetUserWishlist(userId!, language);
            return Ok(result);
        }
    }
}
