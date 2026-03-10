using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] ProductFilterDTO filterDto)
        {
            var language = HttpContext.Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en";
            }
            var products = await _productService.GetAllByLanguage(language, filterDto);
            var count = await _productService.GetCount(language, filterDto);
            var data = new ModelsWithPaginationResponse<ProductResponse>()
            {
                Items = products,
                Pagination = new PaginationResponse()
                {
                    TotalCount = count,
                    PageNumber = filterDto.PageNumber,
                    PageSize = filterDto.PageSize
                }
            };
            return Ok(ApiResponse<ModelsWithPaginationResponse<ProductResponse>>.SuccessResponse(200, "Returned successfully", data));
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromForm] CreateProductRequest request)
        {
            await _productService.Add(request);
            return Ok(ApiResponse<string>.SuccessResponse(200, "Created successfully", null));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] EditProductRequest request)
        {
            await _productService.Edit(id, request);
            return Ok(ApiResponse<string>.SuccessResponse(200, "Updated successfully", null));

        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetById(id);
            return Ok(ApiResponse<ProductWithTranslatedResponse>.SuccessResponse(200, "Returned successfully", product));
        }
        

    }
}
