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
    public interface ICategoryService : IService<Category>
    {
        Task<Category?> GetOneAsync(Expression<Func<Category, bool>>[]? filter = null, CancellationToken cancellationToken = default);
        Task<bool> IsUnique(string name, int id =0, CancellationToken cancellationToken = default);
    }
}
