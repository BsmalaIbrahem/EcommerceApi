using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class BookService : MainService<Book>, IBookService
    {
        public BookService(IRepository<Book> repository) : base(repository)
        {
        }

        public  async Task<ModelsWithPaginationResponse<Book>> GetWithPaginationAsync(FilterBookRequest filter)
        {
            var conditions = new List<Expression<Func<Book, bool>>>();
            if (filter.CategoryId != null)
            {
                conditions.Add(b => b.CategoryId == filter.CategoryId);
            }

            if(filter.Search != null && filter.Search != "")
            {
                var search = filter.Search.ToLower();
                search = search.Trim();
                conditions.Add(x => x.Title.Contains(search) || x.Author.Contains(search));
            }

            Func<IQueryable<Book>, IIncludableQueryable<Book, object>>  includes = x => x.Include(b => b.Category);
            return await base.GetWithPaginationAsync(conditions.ToArray(), filter.paginationRequest.PageNumber, filter.paginationRequest.PageSize, includes);
        }


    }
}
