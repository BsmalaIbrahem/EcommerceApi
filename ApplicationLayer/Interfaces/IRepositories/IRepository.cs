using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace ApplicationLayer.Interfaces.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>,
            IIncludableQueryable<T, object>>? includeChain = null,
            bool AsNoTracking = false);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeChain = null,
            bool asNoTracking = false, int? skip = null, int? take = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
        Task<int> CountAsync(Expression<Func<T, bool>>? expression = null);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Expression<Func<T, bool>> expression);
        Task SaveChangesAsync();
        IQueryable<T> Query();
    }
}
