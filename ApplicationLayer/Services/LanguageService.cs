using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILanguageRepository _languageRepository;
        public LanguageService(ILanguageRepository languageRepository)
        {
            _languageRepository = languageRepository;
        }
        public async Task<Languages> GetByCode(string code)
        {
            var language = await _languageRepository.GetOneAsync([x => x.Code == code]);
            if (language == null)
            {
                throw new Exception($"Language with code '{code}' not found.");
            }
            return language;
        }

        public async Task<IEnumerable<Languages>> GetAllLanguagesAsync()
        {
            return await _languageRepository.GetAllAsync();
        }
    }
}
