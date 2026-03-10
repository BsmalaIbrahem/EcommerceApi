using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using ApplicationLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class AdController : ControllerBase
    {
        private readonly IAdService _adService;

        public AdController(IAdService adService)
        {
            _adService = adService;
        }


        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] AdFilterDTO filterDto)
        {
            var language = HttpContext.Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en";
            }
            var Ads = await _adService.GetAdsByLanguage(language, filterDto);
            var count = await _adService.GetCount(language, filterDto);
            var data = new ModelsWithPaginationResponse<AdResponse>()
            {
                Items = Ads,
                Pagination = new PaginationResponse()
                {
                    TotalCount = count,
                    PageNumber = filterDto.PageNumber,
                    PageSize = filterDto.PageSize
                }
            };
            return Ok(ApiResponse<ModelsWithPaginationResponse<AdResponse>>.SuccessResponse(200, "Returned successfully", data));
        }


        [HttpPost("create")]
        public async Task<IActionResult> Add([FromForm] CreateAdRequest request)
        {
            var translations = new List<AdTranslationDto>();
            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<AdTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                return BadRequest(ApiResponse<object>.FailureResponse(400, "Translations are required.\r\n"));
            }

            await _adService.Add(request);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Added Successfully", null));
        }


    }
}
