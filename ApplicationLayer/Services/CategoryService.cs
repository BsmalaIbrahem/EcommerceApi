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
    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _repository;
        private readonly ILanguageService _languageService;
        private readonly IUrlService _urlService;
        public CategoryService(IRepository<Category> repository, ILanguageService languageService, IUrlService urlService)
        {
            _repository = repository;
            _languageService = languageService;
            _urlService = urlService;
        }
        public async Task Add(CreateCategoryRequest request)
        {
            var translations = new List<CategoryTranslationDto>();

            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<CategoryTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                throw new ArgumentException("Translations are required.");
            }

            var categoryTranslations = new List<CategoryTranslation>();

            foreach (var t in translations)
            {
                var lang = await _languageService.GetByCode(t.LanguageCode);

                categoryTranslations.Add(new CategoryTranslation
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

            var category = new Category
            {
                ImagePath = imagePath,
                CategoryTranslations = categoryTranslations
            };

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
        }


        public async Task Edit(int categoryId, EditCategoryRequest request)
        {
            var category = await _repository.GetOneAsync([x => x.Id == categoryId]);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                var translations = JsonSerializer.Deserialize<List<CategoryTranslationDto>>(request.TranslationsJson);
                if (translations == null || translations.Count == 0)
                    throw new ArgumentException("Translations are required.");

                foreach (var t in translations)
                {
                    var lang = await _languageService.GetByCode(t.LanguageCode);
                    category.CategoryTranslations.Add(new CategoryTranslation
                    {
                        LanguageId = lang.Id,
                        Name = t.Name
                    });
                    
                }
            }
            if (request.Image != null)
            {
                if (!string.IsNullOrEmpty(category.ImagePath))
                {
                    FileHelper.DeleteFile(category.ImagePath);
                }

                var uploadedImage = await FileHelper.UploadAndSaveFileAsync(request.Image!, "images/categories");
                category.ImagePath = uploadedImage.RelativePath;
            }

             _repository.UpdateAsync(category);
            await _repository.SaveChangesAsync();
        }


        public async Task<bool> IsExited(string name, string language, int? categoryId = null)
        {
            var lang = await _languageService.GetByCode(language);
            var filters = new List<System.Linq.Expressions.Expression<Func<Category, bool>>>();
            if (categoryId.HasValue)
            {
                filters.Add(x => x.Id != categoryId.Value);
            }
            filters.Add(x => x.CategoryTranslations.Any(ct => ct.Name == name && ct.LanguageId == lang.Id));
            var existedCategory = await _repository.GetOneAsync(filters);
            if (existedCategory != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllByLanguage(string language)
        {
            var lang = await _languageService.GetByCode(language);
            var categories = await _repository.GetAllAsync(
                includeChain: q => q.Include(c => c.CategoryTranslations.Where(ct => ct.LanguageId == lang.Id))
            );
            var result = categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.CategoryTranslations.First()?.Name ?? "",
                ImagePath = _urlService.GetFullUrl(c.ImagePath ?? null)
            });
            return result;
        }

        public async Task<IEnumerable<CategoryWithTranslatedResponse>> GetAllWithTranslated()
        {
            var categories = await _repository.GetAllAsync(
                includeChain: q => q.Include(c => c.CategoryTranslations)
            );
            var result = categories.Select(c => new CategoryWithTranslatedResponse
            {
                Id = c.Id,
                ImagePath = _urlService.GetFullUrl(c.ImagePath ?? null),
                Translations = c.CategoryTranslations.Select(ct => new CategoryTranslationDto
                {
                    LanguageCode = ct.LanguageCode,
                    Name = ct.Name,
                }).ToList()
            });
            return result;
        }

        public async Task<CategoryWithTranslatedResponse> GetById(int Id)
        {
            var category = await _repository.GetOneAsync(
               includeChain: q => q.Include(c => c.CategoryTranslations).ThenInclude(ct => ct.Language),
               filters: [x => x.Id == Id]
           );
            if (category == null)
                return new CategoryWithTranslatedResponse();

            var result = new CategoryWithTranslatedResponse
            {
                Id = category.Id,
                ImagePath = _urlService.GetFullUrl(category.ImagePath ?? null),
                Translations = category.CategoryTranslations.Select(ct => new CategoryTranslationDto
                {
                    LanguageCode = ct.Language.Code,
                    Name = ct.Name
                }).ToList()
            };

            return result;
        }

        public async Task<CategoryResponse> GetByIdAndLanguage(int Id, string language)
        {

            var lang = await _languageService.GetByCode(language);
            var category = await _repository.GetOneAsync(
                includeChain: q => q.Include(c => c.CategoryTranslations.Where(ct => ct.LanguageId == lang.Id)),
                filters: [x => x.Id == Id]
            );

            var result = new CategoryResponse
            {
                Id = category.Id,
                ImagePath = _urlService.GetFullUrl(category.ImagePath ?? null),
                Name = category.CategoryTranslations.First()?.Name ?? "",
                
            };
            return result;
        }
    }
}
