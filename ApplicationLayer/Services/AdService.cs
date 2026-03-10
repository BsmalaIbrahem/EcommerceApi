using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using SharedLayer.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class AdService : IAdService
    {
        private readonly IRepository<Advertisement> _repository;
        private readonly ILanguageService _languageService;
        private readonly IUrlService _urlService;

        public AdService(IRepository<Advertisement> repository, ILanguageService languageService, IUrlService urlService)
        {
            _repository = repository;
            _languageService = languageService;
            _urlService = urlService;
        }

        public async Task Add(CreateAdRequest request)
        {
            var translations = JsonSerializer.Deserialize<List<AdTranslationDto>>(request.TranslationsJson)
                ?? throw new ArgumentException("Translations required.");

            var adTranslations = new List<AdTranslation>();
            foreach (var t in translations)
            {
                var lang = await _languageService.GetByCode(t.LanguageCode);
                adTranslations.Add(new AdTranslation
                {
                    LanguageId = lang.Id,
                    SmallTitle = t.SmallTitle,
                    MainTitle = t.MainTitle,
                    SubTitle = t.SubTitle
                });
            }

            var upload = await FileHelper.UploadAndSaveFileAsync(request.Image, "images/ads");

            var ad = new Advertisement
            {
                ImagePath = upload.RelativePath,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                ExternalUrl = request.ExternalUrl,
                Priority = request.Priority,
                AdTranslations = adTranslations
            };

            await _repository.AddAsync(ad);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<AdResponse>> GetAdsByLanguage(string language, AdFilterDTO filterDto)
        {
            var lang = await _languageService.GetByCode(language);

            // 1. بناء لستة الفلاتر ديناميكياً
            var filters = new List<Expression<Func<Advertisement, bool>>>();

            // فلتر الحالة (نشط أم لا) - لو مش مبعوت بنجيب النشط فقط كـ Default
            if (filterDto.IsActive.HasValue)
                filters.Add(a => a.IsActive == filterDto.IsActive.Value);
            else
                filters.Add(a => a.IsActive == true);

            // فلتر نوع الهدف (Category, Product, etc.)
            if (filterDto.TargetType.HasValue)
                filters.Add(a => a.TargetType == filterDto.TargetType.Value);

            // البحث في العناوين (Small, Main, Sub) باللغة الحالية
            if (!string.IsNullOrEmpty(filterDto.SearchText))
            {
                filters.Add(a => a.AdTranslations.Any(t =>
                    t.LanguageId == lang.Id &&
                    (t.MainTitle.Contains(filterDto.SearchText) ||
                     t.SmallTitle.Contains(filterDto.SearchText) ||
                     t.SubTitle.Contains(filterDto.SearchText))));
            }

            // 2. تحديد الـ OrderBy (حسب الأولوية Priority)
            Func<IQueryable<Advertisement>, IOrderedQueryable<Advertisement>>? orderBy = null;
            if (filterDto.SortedByPriority == true)
            {
                orderBy = filterDto.SortedByDesc == true
                    ? q => q.OrderByDescending(a => a.Priority)
                    : q => q.OrderBy(a => a.Priority);
            }

            // 3. جلب البيانات المفلترة والمقسمة صفحات
            var ads = await _repository.GetAllAsync(
                filters: filters,
                includeChain: q => q.Include(a => a.AdTranslations.Where(t => t.LanguageId == lang.Id)),
                orderBy: orderBy,
                skip: (filterDto.PageNumber - 1) * filterDto.PageSize,
                take: filterDto.PageSize,
                asNoTracking: true
            );

            // 4. عمل Mapping للـ Response
            return ads.Select(a => new AdResponse
            {
                Id = a.Id,
                ImagePath = _urlService.GetFullUrl(a.ImagePath),
                SmallTitle = a.AdTranslations.FirstOrDefault()?.SmallTitle ?? "",
                MainTitle = a.AdTranslations.FirstOrDefault()?.MainTitle ?? "",
                SubTitle = a.AdTranslations.FirstOrDefault()?.SubTitle ?? "",
                TargetType = a.TargetType,
                TargetId = a.TargetId,
                TargetUrl = BuildTargetUrl(a) // الميثود اللي عملناها قبل كدة
            });
        }


        public async Task<int> GetCount(string language, AdFilterDTO filterDto)
        {
            var lang = await _languageService.GetByCode(language);

            var filters = new List<Expression<Func<Advertisement, bool>>>();



            if (!string.IsNullOrEmpty(filterDto.SearchText))
            {
                // البحث في الترجمة بناءً على اللغة الحالية
                filters.Add(p => p.AdTranslations.Any(t =>
                    t.LanguageId == lang.Id && (t.SmallTitle.Contains(filterDto.SearchText) || t.MainTitle.Contains(filterDto.SearchText) || t.SubTitle.Contains(filterDto.SearchText))));
            }

            // 3. حساب الإجمالي بناءً على الفلاتر (مهم جداً للـ Pagination)
            return await _repository.CountAsync(filters.ToArray());
        }


        private string BuildTargetUrl(Advertisement ad) => ad.TargetType switch
        {
            AdTargetType.Category => $"/category/{ad.TargetId}",
            AdTargetType.Product => $"/product/{ad.TargetId}",
            AdTargetType.AllOffers => "/offers",
            _ => ad.ExternalUrl ?? "#"
        };
    }
}
