using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using SharedLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class BrandService : IBrandService
    {
        private readonly IRepository<Brand> _repository;
        private readonly ILanguageService _languageService;
        private readonly IUrlService _urlService;
        public BrandService(IRepository<Brand> repository, ILanguageService languageService, IUrlService urlService)
        {
            _repository = repository;
            _languageService = languageService;
            _urlService = urlService;
        }
        public async Task Add(CreateBrandRequest request)
        {
            var translations = new List<BrandTranslationDto>();

            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<BrandTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                throw new ArgumentException("Translations are required.");
            }

            var BrandTranslations = new List<BrandTranslation>();

            foreach (var t in translations)
            {
                var lang = await _languageService.GetByCode(t.LanguageCode);

                BrandTranslations.Add(new BrandTranslation
                {
                    LanguageId = lang.Id,
                    Name = t.Name
                });
            }

            string? imagePath = null;

            if (request.Image != null)
            {
                var uploadedImage = await FileHelper.UploadAndSaveFileAsync(request.Image!, "images/categories");
                imagePath = uploadedImage.RelativePath;
            }

            var Brand = new Brand
            {
                ImagePath = imagePath,
                BrandTranslations = BrandTranslations
            };

            await _repository.AddAsync(Brand);
            await _repository.SaveChangesAsync();
        }


        public async Task Edit(int BrandId, EditBrandRequest request)
        {
            var Brand = await _repository.GetOneAsync([x => x.Id == BrandId]);
            if (Brand == null)
                throw new KeyNotFoundException("Brand not found.");

            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                var translations = JsonSerializer.Deserialize<List<BrandTranslationDto>>(request.TranslationsJson);
                if (translations == null || translations.Count == 0)
                    throw new ArgumentException("Translations are required.");

                foreach (var t in translations)
                {
                    var lang = await _languageService.GetByCode(t.LanguageCode);
                    Brand.BrandTranslations.Add(new BrandTranslation
                    {
                        LanguageId = lang.Id,
                        Name = t.Name
                    });

                }
            }
            if (request.Image != null)
            {
                if (!string.IsNullOrEmpty(Brand.ImagePath))
                {
                    FileHelper.DeleteFile(Brand.ImagePath);
                }

                var uploadedImage = await FileHelper.UploadAndSaveFileAsync(request.Image!, "images/categories");
                Brand.ImagePath = uploadedImage.RelativePath;
            }

            _repository.UpdateAsync(Brand);
            await _repository.SaveChangesAsync();
        }


        public async Task<bool> IsExited(string name, string language, int? BrandId = null)
        {
            var lang = await _languageService.GetByCode(language);
            var filters = new List<System.Linq.Expressions.Expression<Func<Brand, bool>>>();
            if (BrandId.HasValue)
            {
                filters.Add(x => x.Id != BrandId.Value);
            }
            filters.Add(x => x.BrandTranslations.Any(ct => ct.Name == name && ct.LanguageId == lang.Id));
            var existedBrand = await _repository.GetOneAsync(filters);
            if (existedBrand != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<BrandResponse>> GetAllByLanguage(string language)
        {
            var lang = await _languageService.GetByCode(language);
            var categories = await _repository.GetAllAsync(
                includeChain: q => q.Include(c => c.BrandTranslations.Where(ct => ct.LanguageId == lang.Id))
            );
            var result = categories.Select(c => new BrandResponse
            {
                Id = c.Id,
                Name = c.BrandTranslations.First()?.Name ?? "",
                ImagePath = _urlService.GetFullUrl(c.ImagePath ?? null)
            });
            return result;
        }

        public async Task<IEnumerable<BrandWithTranslatedResponse>> GetAllWithTranslated()
        {
            var categories = await _repository.GetAllAsync(
                includeChain: q => q.Include(c => c.BrandTranslations)
            );
            var result = categories.Select(c => new BrandWithTranslatedResponse
            {
                Id = c.Id,
                ImagePath = _urlService.GetFullUrl(c.ImagePath ?? null),
                Translations = c.BrandTranslations.Select(ct => new BrandTranslationDto
                {
                    LanguageCode = ct.LanguageCode,
                    Name = ct.Name,
                }).ToList()
            });
            return result;
        }

        public async Task<BrandWithTranslatedResponse> GetById(int Id)
        {
            var Brand = await _repository.GetOneAsync(
               includeChain: q => q.Include(c => c.BrandTranslations).ThenInclude(ct => ct.Language),
               filters: [x => x.Id == Id]
           );
            if (Brand == null)
                return new BrandWithTranslatedResponse();

            var result = new BrandWithTranslatedResponse
            {
                Id = Brand.Id,
                ImagePath = _urlService.GetFullUrl(Brand.ImagePath ?? null),
                Translations = Brand.BrandTranslations.Select(ct => new BrandTranslationDto
                {
                    LanguageCode = ct.Language.Code,
                    Name = ct.Name
                }).ToList()
            };

            return result;
        }

        public async Task<BrandResponse> GetByIdAndLanguage(int Id, string language)
        {

            var lang = await _languageService.GetByCode(language);
            var Brand = await _repository.GetOneAsync(
                includeChain: q => q.Include(c => c.BrandTranslations.Where(ct => ct.LanguageId == lang.Id)),
                filters: [x => x.Id == Id]
            );

            var result = new BrandResponse
            {
                Id = Brand.Id,
                ImagePath = _urlService.GetFullUrl(Brand.ImagePath ?? null),
                Name = Brand.BrandTranslations.First()?.Name ?? "",

            };
            return result;
        }
    }
}
