using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PresentationLayer.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var language = HttpContext.Request.Headers["Accept-Language"];
            if (string.IsNullOrEmpty(language))
            {
                var BrandById = await _brandService.GetById(id);
                return Ok(ApiResponse<BrandWithTranslatedResponse>.SuccessResponse(200, "Returned successfully", BrandById));
            }

            var Brand = await _brandService.GetByIdAndLanguage(id, language.ToString());

            return Ok(ApiResponse<BrandResponse>.SuccessResponse(200, "Returned successfully", Brand));
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var language = HttpContext.Request.Headers["Accept-Language"].ToString();
            if (language == "")
            {
                language = "en";
            }
            var categories = await _brandService.GetAllByLanguage(language);
            return Ok(ApiResponse<IEnumerable<BrandResponse>>.SuccessResponse(200, "Returned", categories));
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] CreateBrandRequest request)
        {
            var translations = new List<BrandTranslationDto>();
            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<BrandTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                return BadRequest(ApiResponse<object>.FailureResponse(400, "Translations are required.\r\n"));
            }


            foreach (var t in translations)
            {
                var existed = await _brandService.IsExited(t.Name, t.LanguageCode);
                if (existed)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(400, $"The Brand name '{t.Name}' has already been used for the language '{t.LanguageCode}' and cannot be duplicated.\r\n"));
                }
            }

            await _brandService.Add(request);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Added Successfully", null));
        }


        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] EditBrandRequest request)
        {
            var translations = new List<BrandTranslationDto>();
            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<BrandTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                return BadRequest(ApiResponse<object>.FailureResponse(400, "Translations are required.\r\n"));
            }

            foreach (var t in translations)
            {
                var existed = await _brandService.IsExited(t.Name, t.LanguageCode, id);
                if (existed)
                {
                    return BadRequest(ApiResponse<object>.FailureResponse(400, $"The Brand name '{t.Name}' has already been used for the language '{t.LanguageCode}' and cannot be duplicated.\r\n"));
                }
            }

            await _brandService.Edit(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Edited Successfully", null));
        }

    }
}
