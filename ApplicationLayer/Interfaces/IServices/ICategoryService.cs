using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllByLanguage(string language);
        Task<IEnumerable<CategoryWithTranslatedResponse>> GetAllWithTranslated();
        Task<CategoryWithTranslatedResponse> GetById(int Id);
        Task<CategoryResponse> GetByIdAndLanguage(int Id, string language);
        Task<bool> IsExited(string name, string language, int? categoryId = null);
        Task Add(CreateCategoryRequest request);
        Task Edit(int categoryId, EditCategoryRequest request);
    }
}
