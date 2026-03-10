using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class HomeResponse
    {
        public IEnumerable<AdResponse> Banners { get; set; } = new List<AdResponse>();
        public IEnumerable<CategoryResponse> FeaturedCategories { get; set; } = new List<CategoryResponse>();
        public IEnumerable<ProductResponse> EditorsPick { get; set; } = new List<ProductResponse>();
        public IEnumerable<ProductResponse> BestSellers { get; set; } = new List<ProductResponse>();
        public IEnumerable<PostResponse> LatestNews { get; set; } = new List<PostResponse>();
    }
}
