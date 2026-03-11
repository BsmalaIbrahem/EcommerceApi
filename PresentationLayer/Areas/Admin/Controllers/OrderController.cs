using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IRepository<Order> _orderRepo;
        public OrderController(IOrderService orderService, IRepository<Order> _orderRepo)
        {
            _orderService = orderService;
            this._orderRepo = _orderRepo;

        }

        [HttpGet("all-orders")]
        public async Task<IActionResult> GetAllOrders(
        [FromQuery] OrderStatus? status,
        [FromQuery] bool? isPaid,
        [FromQuery] string? userId)
        {
            var language = HttpContext.Request.Headers["Accept-Language"]!.ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en"; // Default to English if no language is specified
            }

            var orders = await _orderService.GetAdminOrders(status, isPaid, userId, language);
            return Ok(orders);
        }

        [HttpPatch("update-status/{orderId}")]
        public async Task<IActionResult> UpdateStatus(int orderId, [FromBody] UpdateStatusRequest req)
        {
            if (!Enum.IsDefined(typeof(OrderStatus), req.NewStatus))
            {
                return BadRequest(new { message = "Invalid Status Value" });
            }

            var order = await _orderRepo.GetOneAsync([x => x.Id == orderId]);
            if (order == null) return NotFound();

            order.Status = (OrderStatus)req.NewStatus;
            await _orderRepo.SaveChangesAsync();
            return Ok(new { message = "Order status updated" });
        }

    }
}
