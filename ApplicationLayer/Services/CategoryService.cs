using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class CategoryService : MainService<Category>, ICategoryService
    {
        public CategoryService(IRepository<Category> repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
        {
        }

        public Task<Category?> GetOneAsync(Expression<Func<Category, bool>>[]? filter = null, CancellationToken cancellationToken = default)
        {
            return base.GetOneAsync(filter, includes: x=>x.Include(q => q.Books), cancellationToken);
        }

        public async Task<bool> IsUnique(string name, int id = 0, CancellationToken cancellationToken = default)
        {
            Expression<Func<Category, bool>>[]? filter = null;
            if(id > 0)
            {
                filter = [c => c.Name == name && c.Id != id];
            }
            else
            {
                filter = [c => c.Name == name];
            }

            var existingCategory = await GetOneAsync(filter: filter, cancellationToken);
            return existingCategory == null;
        }
    }
}
