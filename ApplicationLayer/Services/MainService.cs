using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
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
    public class MainService<T> : IService<T> where T : class
    {
        private readonly IRepository<T> _repository;
        public MainService(IRepository<T> repository) 
        {
            _repository = repository;
        }

        public async Task CreateAsync(T entity)
        {
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(Expression<Func<T, bool>> filter)
        {
            await _repository.DeleteAsync(filter);
            await _repository.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            var exists = await _repository.GetOneAsync([x => EF.Property<int>(x, "Id") == id]);
            return exists != null;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>[]? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
        {
            var data = await _repository.GetAllAsync(filters: filter, includeChain: includes);
            return data;
        }

        public async Task<T?> GetOneAsync(Expression<Func<T, bool>>[]? filter = null)
        {
            var data = await _repository.GetOneAsync(filter);
            if (data == null)
            {
                throw new KeyNotFoundException($"Entity not found.");
            }
            return data;
        }

        public async Task<ModelsWithPaginationResponse<T>> GetWithPaginationAsync(Expression<Func<T, bool>>[]? filter = null, int pageNumber = 1, int PageSize = 5, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null)
        {
            var skip = (pageNumber - 1) * PageSize;
            var items = await _repository.GetAllAsync(filters: filter, skip: skip, take: PageSize, includeChain: includes);
           
            var totalCount = await _repository.CountAsync(filter);

            var data = new ModelsWithPaginationResponse<T>
            {
                Items = items,
                Pagination = new PaginationResponse
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = PageSize,
                }
            };
            return data;
        }

        public async Task UpdateAsync(T entity)
        {
            _repository.UpdateAsync(entity);
           await _repository.SaveChangesAsync();
           
        }
    }
}
