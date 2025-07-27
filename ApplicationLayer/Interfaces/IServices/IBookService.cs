using ApplicationLayer.DTOs;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IBookService : IService<Book>
    {
        Task<ModelsWithPaginationResponse<Book>> GetWithPaginationAsync(FilterBookRequest filter);
    }
}
