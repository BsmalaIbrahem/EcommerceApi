using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLayer.Utility;
using System.Text.Json;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var language = HttpContext.Request.Headers["Accept-Language"];
            if(string.IsNullOrEmpty(language))
            {
                var categoryById = await _categoryService.GetById(id);
                return Ok(ApiResponse<CategoryWithTranslatedResponse>.SuccessResponse(200, "Returned successfully", categoryById));
            }

            var category = await _categoryService.GetByIdAndLanguage(id, language.ToString());

            return Ok(ApiResponse<CategoryResponse>.SuccessResponse(200, "Returned successfully", category));
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var language = HttpContext.Request.Headers["Accept-Language"].ToString();
            if(language == "")
            {
                language = "en";
            }
            var categories = await _categoryService.GetAllByLanguage(language);
            return Ok(ApiResponse<IEnumerable<CategoryResponse>>.SuccessResponse(200, "Returned", categories));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateCategoryRequest request)
        {
            var translations = new List<CategoryTranslationDto>();
            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<CategoryTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                return BadRequest(ApiResponse<object>.FailureResponse(400, "Translations are required.\r\n"));
            }


            foreach (var t in translations)
            {
                var existed = await _categoryService.IsExited(t.Name, t.LanguageCode);
                if (existed)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(400, $"The category name '{t.Name}' has already been used for the language '{t.LanguageCode}' and cannot be duplicated.\r\n"));
                }
            }

            await _categoryService.Add(request);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Added Successfully", null));
        }


        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] EditCategoryRequest request)
        {
            var translations = new List<CategoryTranslationDto>();
            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<CategoryTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                return BadRequest(ApiResponse<object>.FailureResponse(400, "Translations are required.\r\n"));
            }

            foreach (var t in translations)
            {
                var existed = await _categoryService.IsExited(t.Name, t.LanguageCode, id);
                if (existed)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(400, $"The category name '{t.Name}' has already been used for the language '{t.LanguageCode}' and cannot be duplicated.\r\n"));
                }
            }

            await _categoryService.Edit(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Edited Successfully", null));
        }


    }
}
