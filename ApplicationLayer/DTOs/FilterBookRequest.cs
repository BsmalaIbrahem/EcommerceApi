using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class FilterBookRequest
    {
        public int? CategoryId { get; set; }
        public string? Search { get; set; }
        public PaginationRequest paginationRequest { get; set; } = new PaginationRequest();
    }
}
