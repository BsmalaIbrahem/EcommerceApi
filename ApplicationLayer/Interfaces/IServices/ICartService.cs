using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface ICartService
    {
        Task AddOrUpdateItem(string userId, int productId, int quantity);
        Task UpdateQuantity(string userId, int productId, int delta); // الـ Delta هو الزيادة أو النقصان (+1 أو -1)
        Task RemoveItem(string userId, int productId);
        Task<CartResponse> GetUserCart(string userId, string language);
        Task MergeGuestCartIntoUserCart(string userId, List<CartItemRequest> guestItems);
        Task<int> GetCartItemsCount(string userId);
    }
}
