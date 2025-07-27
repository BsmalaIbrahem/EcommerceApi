using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLayer.Helpers;
using System.Threading.Tasks;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookService _service;
        public BookController(IBookService service) 
        {
            _service = service;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(FilterBookRequest filter)
        {
            var books = await _service.GetWithPaginationAsync(filter);
            return Json(new
            {
                success = true,
                message = "Books retrieved successfully",
                data = books
            });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }
            if (!await _service.ExistsAsync(id))
            {
                return NotFound($"Book with ID: {id} not found.");
            }
            var book = await _service.GetOneAsync([b => b.Id == id]);

            return Json(new {success = true, Message = "Details", Data = book});
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.ImgPath == null || request.ImgPath.Length == 0)
            {
                return BadRequest("Image file is required.");
            }

            string fileName = FileHelper.CreateFileName(request.ImgPath.FileName);
            var path = "assets\\images\\books";
            string filePath = FileHelper.GetFilePath(fileName, "wwwroot\\" + path);
            await FileHelper.UploadFile(filePath, request.ImgPath);

            await _service.CreateAsync(new Book
            {
                CategoryId = request.CategoryId,
                Title = request.Title,
                Description = request.Description,
                Author = request.Author,
                PublishedDate = request.PublishedDate,
                ISBN = request.ISBN,
                PageCount = request.PageCount,
                Price = request.Price,
                ImgPath = path + '\\' + fileName,
                Quantity = request.Quantity
            });

            return Json(new { success = true, message = "Created" });
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, UpdateBookRequest request)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid book ID.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!await _service.ExistsAsync(id))
            {
                return NotFound($"Book with ID: {id} not found.");
            }
            var book = new Book
            {
                Id = id,
                CategoryId = request.CategoryId,
                Title = request.Title,
                Description = request.Description,
                Author = request.Author,
                PublishedDate = request.PublishedDate,
                ISBN = request.ISBN,
                PageCount = request.PageCount,
                Price = request.Price,
                Quantity = request.Quantity
            };

            if (request.ImgPath != null || request.ImgPath.Length >  0)
            {
                string fileName = FileHelper.CreateFileName(request.ImgPath.FileName);
                var path = "assets\\images\\books";
                string filePath = FileHelper.GetFilePath(fileName, "wwwroot\\" + path);
                await FileHelper.UploadFile(filePath, request.ImgPath);
                book.ImgPath = path + '\\' + fileName;
            }

            await _service.UpdateAsync(book);
            return Ok($"Book with ID: {id} updated successfully.");
        }


    }
}
