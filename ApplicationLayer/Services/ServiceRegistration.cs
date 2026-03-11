using ApplicationLayer.Interfaces.IServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IAdService, AdService>();
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IWishlistService, WishlistService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<ICartService, CartService>();

            return services;
        }
    }
}
