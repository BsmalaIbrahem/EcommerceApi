using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IWishlistService
    {
        Task ToggleWishlist(string userId, int productId);
        Task<IEnumerable<ProductResponse>> GetUserWishlist(string userId, string language);
        Task<bool> IsInWishlist(string userId, int productId);
    }
}
