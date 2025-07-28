using ApplicationLayer.DTOs;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IService<T> where T : class
    {
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>[]? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null);
        Task<ModelsWithPaginationResponse<T>> GetWithPaginationAsync(Expression<Func<T, bool>>[]? filter = null, int pageNumber = 1, int PageSize=5, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null);
        Task<T?> GetOneAsync(Expression<Func<T, bool>>[]? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Expression<Func<T, bool>> filter);
        Task<bool> ExistsAsync(int id);
    }
}
