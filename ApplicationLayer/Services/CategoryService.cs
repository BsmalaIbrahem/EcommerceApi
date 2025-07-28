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
        public CategoryService(IRepository<Category> repository) : base(repository)
        {
        }

        public Task<Category?> GetOneAsync(Expression<Func<Category, bool>>[]? filter = null)
        {
            return base.GetOneAsync(filter, includes: x=>x.Include(q => q.Books));
        }

        public async Task<bool> IsUnique(string name, int id = 0)
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

            var existingCategory = await GetOneAsync(filter: filter);
            return existingCategory == null;
        }
    }
}
