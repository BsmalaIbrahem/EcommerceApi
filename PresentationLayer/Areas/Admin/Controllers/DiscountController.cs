using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using ApplicationLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDiscountRequest request)
        {
            await _discountService.CreateDiscount(request);
            return Ok(ApiResponse<string>.SuccessResponse(200, "Created successfully", null));
        }
    }
}
