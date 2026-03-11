using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _cartRepo;
        private readonly IProductService _productService;

        public CartService(IRepository<Cart> cartRepo, IProductService productService)
        {
            _cartRepo = cartRepo;
            _productService = productService;
        }

        // 1. جلب السلة (بتحسب الأسعار والخصومات لحظياً)
        public async Task<CartResponse> GetUserCart(string userId, string language)
        {
            var cart = await _cartRepo.GetOneAsync(
                filters: [c => c.UserId == userId],
                includeChain: q => q.Include(c => c.CartItems).ThenInclude(ci => ci.Product)
            );

            if (cart == null) return new CartResponse();

            var response = new CartResponse();
            var items = new List<CartItemResponse>();

            foreach (var ci in cart.CartItems)
            {
                // بنستخدم الـ ProductService اللي عملناه عشان نحسب الخصومات الحالية
                var productData = await _productService.GetByIdAndLanguage(ci.ProductId, language);

                items.Add(new CartItemResponse
                {
                    ProductId = ci.ProductId,
                    ProductName = productData.Name,
                    ImagePath = productData.ImagePath,
                    Quantity = ci.Quantity,
                    UnitPrice = productData.Price,
                    FinalPrice = productData.FinalPrice
                });
            }
            response.Items = items;
            return response;
        }

        // 2. إضافة/تعديل كمية
        public async Task AddOrUpdateItem(string userId, int productId, int quantity)
        {
            var cart = await GetOrCreateCart(userId);
            var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity = quantity > 0 ? quantity : 1;
            }
            else
            {
                cart.CartItems.Add(new CartItem { ProductId = productId, Quantity = quantity });
            }
            await _cartRepo.SaveChangesAsync();
        }

        // 3. الـ Merge Logic (المهم جداً)
        // بينادى عليه أول ما المستخدم يعمل Login
        public async Task MergeGuestCartIntoUserCart(string userId, List<CartItemRequest> guestItems)
        {
            var userCart = await GetOrCreateCart(userId);

            foreach (var guestItem in guestItems)
            {
                var existingItem = userCart.CartItems.FirstOrDefault(ci => ci.ProductId == guestItem.ProductId);
                if (existingItem != null)
                {
                    // لو المنتج موجود في الاتنين، بنزود الكمية أو ناخد الأكبر (حسب البزنس)
                    existingItem.Quantity += guestItem.Quantity;
                }
                else
                {
                    userCart.CartItems.Add(new CartItem
                    {
                        ProductId = guestItem.ProductId,
                        Quantity = guestItem.Quantity
                    });
                }
            }
            await _cartRepo.SaveChangesAsync();
        }

        private async Task<Cart> GetOrCreateCart(string userId)
        {
            var cart = await _cartRepo.GetOneAsync(
                filters: [c => c.UserId == userId],
                includeChain: q => q.Include(c => c.CartItems)
            );

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _cartRepo.AddAsync(cart);
                await _cartRepo.SaveChangesAsync();
            }
            return cart;
        }

        public async Task UpdateQuantity(string userId, int productId, int delta)
        {
            var cart = await GetOrCreateCart(userId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (item != null)
            {
                item.Quantity += delta;

                // لو الكمية وصلت لصفر أو أقل، بنمسح المنتج من السلة خالص
                if (item.Quantity <= 0)
                {
                    cart.CartItems.Remove(item);
                }

                await _cartRepo.SaveChangesAsync();
            }
        }

        public async Task RemoveItem(string userId, int productId)
        {
            var cart = await GetOrCreateCart(userId);
            var item = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (item != null)
            {
                cart.CartItems.Remove(item);
                await _cartRepo.SaveChangesAsync();
            }
        }

        public async Task<int> GetCartItemsCount(string userId)
        {
            var cart = await _cartRepo.GetOneAsync(
                filters: [c => c.UserId == userId],
                includeChain: q => q.Include(c => c.CartItems)
            );

            if (cart == null) return 0;

            // بنجمع الكميات (يعني لو عندي 2 تفاح و 1 موز، يظهر رقم 3)
            return cart.CartItems.Sum(ci => ci.Quantity);
        }

    }
}
