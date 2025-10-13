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
        Task<T?> GetOneAsync(IEnumerable<Expression<Func<T, bool>>>? filters = null, Func<IQueryable<T>,
            IIncludableQueryable<T, object>>? includeChain = null,
            bool AsNoTracking = false, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(IEnumerable<Expression<Func<T, bool>>>? filters = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeChain = null,
            bool asNoTracking = false, int? skip = null, int? take = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>>[]? expression = null);
        Task AddAsync(T entity);
        void UpdateAsync(T entity);
        Task DeleteAsync(Expression<Func<T, bool>> expression);
       
        IQueryable<T> Query();
    }
}
