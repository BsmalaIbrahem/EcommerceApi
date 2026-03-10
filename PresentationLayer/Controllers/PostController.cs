using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using ApplicationLayer.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PresentationLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var language = HttpContext.Request.Headers["Accept-Language"];
            if (string.IsNullOrEmpty(language))
            {
               language = "en";
            }

            var post = await _postService.GetByIdAndLanguage(id, language);

            return Ok(ApiResponse<PostResponse>.SuccessResponse(200, "Returned successfully", post));
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll([FromQuery] PostFilterDTO filterDto)
        {
            var language = HttpContext.Request.Headers["Accept-Language"].ToString();
            if (string.IsNullOrEmpty(language))
            {
                language = "en";
            }
            var Posts = await _postService.GetAllByLanguage(language, filterDto);
            var count = await _postService.GetCount(language, filterDto);
            var data = new ModelsWithPaginationResponse<PostResponse>()
            {
                Items = Posts,
                Pagination = new PaginationResponse()
                {
                    TotalCount = count,
                    PageNumber = filterDto.PageNumber,
                    PageSize = filterDto.PageSize
                }
            };
            return Ok(ApiResponse<ModelsWithPaginationResponse<PostResponse>>.SuccessResponse(200, "Returned successfully", data));
        }

    }
}
