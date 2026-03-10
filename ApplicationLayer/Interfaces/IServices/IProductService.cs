using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IProductService
    {
        Task Add(CreateProductRequest request);
        Task Edit(int id, EditProductRequest request);
        Task<IEnumerable<ProductResponse>> GetAllByLanguage(string language, ProductFilterDTO filterDto);
        Task<ProductWithTranslatedResponse> GetById(int Id);
        Task<ProductResponse> GetByIdAndLanguage(int Id, string language);
        Task<int> GetCount(string language, ProductFilterDTO filterDto);
    }
}
