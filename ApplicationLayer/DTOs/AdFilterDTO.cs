using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs
{
    public class AdFilterDTO
    {
        public string? SearchText { get; set; }
        public AdTargetType? TargetType { get; set; }
        public bool? IsActive { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? SortedByPriority { get; set; } = true;
        public bool? SortedByDesc { get; set; } = false;
    }
}
