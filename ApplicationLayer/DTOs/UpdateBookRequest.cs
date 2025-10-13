using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public record UpdateBookRequest
    {
        [Required(ErrorMessage = "Category ID is required.")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; } = string.Empty;
        [Required(ErrorMessage = "Author is required.")]
        public string Author { get; set; } = string.Empty;
        public DateTime? PublishedDate { get; set; }
        [Required(ErrorMessage = "ISBN is required.")]
        public string ISBN { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Page count must be a positive number.")]
        public int PageCount { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public double Price { get; set; }
        [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Please upload a valid image file (jpg, jpeg, png, gif).")]
        public IFormFile? ImgPath { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative number.")]
        public int Quantity { get; set; }
    }
}
