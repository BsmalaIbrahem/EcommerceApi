using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IAdService
    {
        Task Add(CreateAdRequest request);
        Task<IEnumerable<AdResponse>> GetAdsByLanguage(string language, AdFilterDTO filterDto);
        Task<int> GetCount(string language, AdFilterDTO filterDto);
    }
}
