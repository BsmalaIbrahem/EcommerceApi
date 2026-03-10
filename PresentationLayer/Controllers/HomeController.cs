using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;
        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet("page-data")]
        public async Task<IActionResult> GetHomePageData()
        {
            var language = HttpContext.Request.Headers["Accept-Language"]!.ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en"; // Default to English if no language is specified
            }
            var homeData = await _homeService.GetHomePageData(language);
            return Ok(ApiResponse<HomeResponse>.SuccessResponse(200, "Home page data retrieved successfully", homeData));
        }
    }
}
