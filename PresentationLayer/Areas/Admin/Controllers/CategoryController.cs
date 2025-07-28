using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace PresentationLayer.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service) 
        {
            _service = service;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var categories = await _service.GetAsync();
            return Json(new
            {
                success = true,
                message = "Categories retrieved successfully",
                data = categories
            });
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return Json(new {success = false, message = "id must be greater than 0", });
            }

            if (!await _service.ExistsAsync(id))
            {
                return Json(new {success = false, message = $"Category with ID: {id} not found."});
            }

            var category = await _service.GetOneAsync([c => c.Id == id]);
            return Json(new {success = true, message = "Details", data = category});
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!await _service.IsUnique(request.Name))
            {
                return Json(new {success = false, message = "Category name must be unique."});
            }
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description
            };
            await _service.CreateAsync(category);
            return Json(new {success = true, message = "Category created successfully", data = category});
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryRequest request)
        {
            if (id <= 0)
            {
                return Json(new {success = false, message = "id must be greater than 0", });
            }
            if (!await _service.ExistsAsync(id))
            {
                return Json(new {success = false, message = $"Category with ID: {id} not found."});
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!await _service.IsUnique(request.Name, id))
            {
                return Json(new {success = false, message = "Category name must be unique."});
            }
            var category = new Category
            {
                Id = id,
                Name = request.Name,
                Description = request.Description
            };
            
            await _service.UpdateAsync(category);
            return Json(new {success = true, message = "Category updated successfully", data = category});
        }
    }
}
