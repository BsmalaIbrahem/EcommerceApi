using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        // 1. جلب كل المنتجات مع الفلترة والـ Pagination
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDTO filter)
        {
            var language = HttpContext.Request.Headers["Accept-Language"]!.ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en"; // Default to English if no language is specified
            }

            var products = await _productService.GetAllByLanguage(language, filter);
            var totalCount = await _productService.GetCount(language, filter);

            // بنرجع الداتا ومعاها الإجمالي عشان الـ Front-end يعرف يعمل صفحات
            var data = new ModelsWithPaginationResponse<ProductResponse>()
            {
                Items = products,
                Pagination = new PaginationResponse()
                {
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize
                }
            };
            return Ok(ApiResponse<ModelsWithPaginationResponse<ProductResponse>>.SuccessResponse(200, "Returned successfully", data));
        }

        // 2. عرض تفاصيل منتج واحد + منتجات مقترحة
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var language = HttpContext.Request.Headers["Accept-Language"]!.ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en"; // Default to English if no language is specified
            }

            var product = await _productService.GetByIdAndLanguage(id, language);

            if (product == null) return NotFound("Product not found");

            // جلب منتجات مقترحة (Recommended)
            // اللوجيك: بنجيب منتجات من نفس القسم (CategoryId) بس بنستبعد المنتج الحالي
            // بنبعت PageSize = 4 زي ما طلبتي
            var recommendedFilter = new ProductFilterDTO
            {
                CategoryId = (await _productService.GetById(id)).CategoryId,
                PageSize = 4
            };

            var allRelated = await _productService.GetAllByLanguage(language, recommendedFilter);
            var recommended = allRelated.Where(p => p.Id != id).Take(4);

            product.RecommendedProducts = recommended.ToList();

            return Ok(ApiResponse<ProductResponse>.SuccessResponse(200, "Returned successfully", product));
        }

            
        
    }
}
