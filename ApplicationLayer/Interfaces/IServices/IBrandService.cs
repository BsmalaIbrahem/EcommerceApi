using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandResponse>> GetAllByLanguage(string language);
        Task<IEnumerable<BrandWithTranslatedResponse>> GetAllWithTranslated();
        Task<BrandWithTranslatedResponse> GetById(int Id);
        Task<BrandResponse> GetByIdAndLanguage(int Id, string language);
        Task<bool> IsExited(string name, string language, int? BrandId = null);
        Task Add(CreateBrandRequest request);
        Task Edit(int BrandId, EditBrandRequest request);
    }
}
