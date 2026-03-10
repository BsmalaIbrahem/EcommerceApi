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
                var postId = await _postService.GetById(id);
                return Ok(ApiResponse<PostWithTranslatedResponse>.SuccessResponse(200, "Returned successfully", postId));
            }

            var post = await _postService.GetById(id);

            return Ok(ApiResponse<PostWithTranslatedResponse>.SuccessResponse(200, "Returned successfully", post));
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


        [HttpPost("create")]
        public async Task<IActionResult> Add([FromForm] CreatePostRequest request)
        {
            var translations = new List<PostTranslationDto>();
            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<PostTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                return BadRequest(ApiResponse<object>.FailureResponse(400, "Translations are required.\r\n"));
            }

            await _postService.Add(request);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Added Successfully", null));
        }


        [HttpPost("edit/{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] EditPostRequest request)
        {
            var translations = new List<PostTranslationDto>();
            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<PostTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                return BadRequest(ApiResponse<object>.FailureResponse(400, "Translations are required.\r\n"));
            }

           
            await _postService.Edit(id, request);
            return Ok(ApiResponse<object>.SuccessResponse(200, "Edited Successfully", null));
        }

    }
}
