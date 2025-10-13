using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public record ModelsWithPaginationResponse<T> where T : class
    {
        public IEnumerable<T>? Items { get; set; }
        public PaginationResponse Pagination { get; set; } = null!;
    }
}
