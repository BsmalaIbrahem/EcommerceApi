using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IRepository<Wishlist> _wishlistRepo;
        private readonly IProductService _productService;

        public WishlistService(IRepository<Wishlist> wishlistRepo, IProductService productService)
        {
            _wishlistRepo = wishlistRepo;
            _productService = productService;
        }

        // ميثود ذكية: لو المنتج موجود بتمسحه، لو مش موجود بتضيفه
        public async Task ToggleWishlist(string userId, int productId)
        {
            var item = await _wishlistRepo.GetOneAsync(filters: [w => w.UserId == userId && w.ProductId == productId]);

            if (item != null)
            {
                await _wishlistRepo.DeleteAsync(x => x.Id == item.Id);
            }
            else
            {
                await _wishlistRepo.AddAsync(new Wishlist { UserId = userId, ProductId = productId });
            }
            await _wishlistRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductResponse>> GetUserWishlist(string userId, string language)
        {
            var wishlistItems = await _wishlistRepo.GetAllAsync(filters: [w => w.UserId == userId]);

            var productList = new List<ProductResponse>();
            foreach (var item in wishlistItems)
            {
                // بنستخدم الـ ProductService عشان يرجعلنا بيانات المنتج بالخصومات والأسعار النهائية
                var product = await _productService.GetByIdAndLanguage(item.ProductId, language);
                if (product != null) productList.Add(product);
            }
            return productList;
        }

        public async Task<bool> IsInWishlist(string userId, int productId)
        {
            var data = await  _wishlistRepo.GetOneAsync([w => w.UserId == userId && w.ProductId == productId]);
            if(data != null) return true;
            return false;
        }
    }
}
