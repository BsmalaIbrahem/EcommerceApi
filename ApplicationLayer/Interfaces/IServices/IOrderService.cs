using ApplicationLayer.DTOs;
using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IOrderService
    {
        Task<string> PlaceOrder(string userId, CheckoutRequest request, string language);
        Task<IEnumerable<OrderResponse>> GetUserOrders(string userId, string lang);
        Task<IEnumerable<OrderResponse>> GetAdminOrders(OrderStatus? status, bool? isPaid, string? userId, string lang);
    }
}
