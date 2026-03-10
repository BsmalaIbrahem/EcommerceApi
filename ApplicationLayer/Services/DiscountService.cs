using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IRepository<Discount> _discountRepository;
        private readonly ILanguageService _languageService;

        public DiscountService(IRepository<Discount> discountRepository, ILanguageService languageService)
        {
            _discountRepository = discountRepository;
            _languageService = languageService;
        }

        public async Task CreateDiscount(CreateDiscountRequest request)
        {
            var translations = JsonSerializer.Deserialize<List<DiscountTranslationDto>>(request.TranslationsJson)
                ?? throw new ArgumentException("Translations are required.");

            var discount = new Discount
            {
                DiscountValue = request.DiscountValue,
                IsPercentage = request.IsPercentage,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsFlashSale = request.IsFlashSale,
                //DiscountTranslations = translations.Select(t => new DiscountTranslation
                //{
                //    // كود جلب الـ LanguageId كما فعلنا سابقاً
                //    Name = t.Name,
                //    Description = t.Description
                //}).ToList()
            };
            var discountTranslations = new List<DiscountTranslation>();
            foreach (var translation in translations) 
            {
                var language = await _languageService.GetByCode(translation.LanguageCode);
                if (language == null)
                {
                    throw new ArgumentException($"Language with code '{translation.LanguageCode}' not found.");
                }
                discountTranslations.Add(new DiscountTranslation
                {
                    LanguageId = language.Id,
                    Name = translation.Name,
                    Description = translation.Description
                });
            }

            // إضافة الربط مع المنتجات
            if (request.ProductIds != null)
            {
                discount.DiscountProducts = request.ProductIds.Select(id => new DiscountProduct { ProductId = id }).ToList();
            }

            // إضافة الربط مع الأقسام
            if (request.CategoryIds != null)
            {
                discount.DiscountCategories = request.CategoryIds.Select(id => new DiscountCategory { CategoryId = id }).ToList();
            }

            await _discountRepository.AddAsync(discount);
            await _discountRepository.SaveChangesAsync();
        }

    }
}
