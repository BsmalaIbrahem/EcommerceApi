using ApplicationLayer.Interfaces.IRepositories;
using InfrastructureLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureLayer.Repositories
{
    public class MainRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        public MainRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<T>> GetAllAsync(IEnumerable<Expression<Func<T, bool>>>? filters = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeChain = null,
            bool asNoTracking = false, int? skip = null, int? take = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            CancellationToken cancellationToken = default
        )
        {
            IQueryable<T> query = _context.Set<T>();

            if (filters != null)
            {
                foreach (var filter in filters)
                    query = query.Where(filter);
            }

            if (includeChain != null)
                query = includeChain(query);

            if (asNoTracking)
                query = query.AsNoTracking();

            if (orderBy != null)
                query = orderBy(query);

            if (skip.HasValue)
                query = query.Skip(skip.Value);
            if(take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetOneAsync(
            IEnumerable<Expression<Func<T, bool>>>? filters = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeChain = null,
            bool asNoTracking = false, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _context.Set<T>();

            if (filters != null)
            {
                foreach (var filter in filters)
                    query = query.Where(filter);
            }

            if (includeChain != null)
                query = includeChain(query);

            if (asNoTracking)
                query = query.AsNoTracking();

            if (orderBy != null)
                query = orderBy(query);

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>[]? expression = null)
        {
            var query = _context.Set<T>().AsQueryable();
            if (expression != null)
            {
                foreach (var filter in expression)
                    query = query.Where(filter);
            }
            return await query.CountAsync();
        }

        public async Task AddAsync(T entity)
        {
            try
            {
                await _context.Set<T>().AddAsync(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the entity.", ex);
            }

        }
        public void UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while Updating the entity.", ex);
            }
        }
        public async Task DeleteAsync(Expression<Func<T, bool>> expression)
        {
            var item = await _context.Set<T>().FirstOrDefaultAsync(expression);
            if (item == null)
            {
                throw new Exception("Entity not found.");
            }
            try
            {
                _context.Set<T>().Remove(item);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the entity.", ex);
            }
        }
      
        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }

        
    }
}
