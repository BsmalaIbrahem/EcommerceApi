using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class HomeService : IHomeService
    {
        private readonly IAdService _adService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IPostService _postService;

        public HomeService(IAdService adService, ICategoryService categoryService,
                           IProductService productService, IPostService postService)
        {
            _adService = adService;
            _categoryService = categoryService;
            _productService = productService;
            _postService = postService;
        }

        public async Task<HomeResponse> GetHomePageData(string language)
        {
            // جلب الإعلانات (البانرات)
            var banners = await _adService.GetAdsByLanguage(language, new AdFilterDTO { PageSize = 5 });

            // جلب الأقسام (الأيقونات الدائرية)
            var categories = await _categoryService.GetAllByLanguage(language);

            // جلب منتجات مختارة (مثلاً أول 6 منتجات أورجانيك)
            var editorsPick = await _productService.GetAllByLanguage(language,
                new ProductFilterDTO { IsOrganic = true, PageSize = 6 });

            // جلب الأكثر مبيعاً (بناءً على الترتيب أو فلتر معين عندك)
            var bestSellers = await _productService.GetAllByLanguage(language,
                new ProductFilterDTO { PageSize = 6, SortedByPrice = true });

            // جلب آخر 4 مقالات للـ "Our News"
            var news = await _postService.GetAllByLanguage(language,
                new PostFilterDTO { PageSize = 4, SortedByDesc = true });

            return new HomeResponse
            {
                Banners = banners,
                FeaturedCategories = categories,
                EditorsPick = editorsPick,
                BestSellers = bestSellers,
                LatestNews = news
            };
        }
    }
}
