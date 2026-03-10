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
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;
        private readonly IRepository<Discount> _discountRepository;
        private readonly ILanguageService _languageService;
        private readonly IUrlService _urlService;

        public ProductService(
            IRepository<Product> repository,
            IRepository<Discount> discountRepository,
            ILanguageService languageService,
            IUrlService urlService)
        {
            _repository = repository;
            _discountRepository = discountRepository;
            _languageService = languageService;
            _urlService = urlService;
        }

        public async Task Add(CreateProductRequest request)
        {
            var translations = JsonSerializer.Deserialize<List<ProductTranslationDto>>(request.TranslationsJson)
                ?? throw new ArgumentException("Translations are required.");

            var productTranslations = new List<ProductTranslation>();
            foreach (var t in translations)
            {
                var lang = await _languageService.GetByCode(t.LanguageCode);
                productTranslations.Add(new ProductTranslation
                {
                    LanguageId = lang.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Unit = t.Unit
                });
            }

            string? imagePath = null;
            if (request.Image != null)
            {
                var uploadedImage = await FileHelper.UploadAndSaveFileAsync(request.Image, "images/products");
                imagePath = uploadedImage.RelativePath;
            }

            var product = new Product
            {
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                IsOrganic = request.IsOrganic,
                CategoryId = request.CategoryId,
                BrandId = request.BrandId,
                ImagePath = imagePath,
                ProductTranslations = productTranslations
            };

            await _repository.AddAsync(product);
            await _repository.SaveChangesAsync();
        }

        public async Task Edit(int productId, EditProductRequest request)
        {
            var product = await _repository.GetOneAsync(
                filters: [x => x.Id == productId],
                includeChain: q => q.Include(p => p.ProductTranslations)
            ) ?? throw new KeyNotFoundException("Product not found.");

            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                var translations = JsonSerializer.Deserialize<List<ProductTranslationDto>>(request.TranslationsJson);
                product.ProductTranslations.Clear(); // بمسح القديم وبنزل الجديد أو ممكن تعمل Logic تحديث
                foreach (var t in translations!)
                {
                    var lang = await _languageService.GetByCode(t.LanguageCode);
                    product.ProductTranslations.Add(new ProductTranslation
                    {
                        LanguageId = lang.Id,
                        Name = t.Name,
                        Description = t.Description,
                        Unit = t.Unit
                    });
                }
            }

            if (request.Image != null)
            {
                if (!string.IsNullOrEmpty(product.ImagePath)) FileHelper.DeleteFile(product.ImagePath);
                var uploadedImage = await FileHelper.UploadAndSaveFileAsync(request.Image, "images/products");
                product.ImagePath = uploadedImage.RelativePath;
            }

            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.IsOrganic = request.IsOrganic;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;

            _repository.UpdateAsync(product);
            await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductResponse>> GetAllByLanguage(string language, ProductFilterDTO filterDto)
        {
            var lang = await _languageService.GetByCode(language);

            // 1. بناء لستة الفلاتر ديناميكياً
            var filters = new List<Expression<Func<Product, bool>>>();

            if (filterDto.CategoryId.HasValue)
                filters.Add(p => p.CategoryId == filterDto.CategoryId.Value);

            if (filterDto.BrandId.HasValue)
                filters.Add(p => p.BrandId == filterDto.BrandId.Value);

            if (filterDto.IsOrganic.HasValue)
                filters.Add(p => p.IsOrganic == filterDto.IsOrganic.Value);

            if (!string.IsNullOrEmpty(filterDto.SearchText))
            {
                // البحث في الترجمة بناءً على اللغة الحالية
                filters.Add(p => p.ProductTranslations.Any(t =>
                    t.LanguageId == lang.Id && t.Name.Contains(filterDto.SearchText)));
            }

            // 2. تحديد الـ OrderBy
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null;
            if (filterDto.SortedByPrice == true)
            {
                orderBy = filterDto.sortedByDesc == true
                    ? q => q.OrderByDescending(p => p.Price)
                    : q => q.OrderBy(p => p.Price);
            }

            // 3. حساب الإجمالي بناءً على الفلاتر (مهم جداً للـ Pagination)
            //var totalCount = await _repository.CountAsync(filters.ToArray());

            // 4. جلب البيانات المفلترة والمقسمة صفحات
            var products = await _repository.GetAllAsync(
                filters: filters,
                includeChain: q => q.Include(p => p.ProductTranslations.Where(t => t.LanguageId == lang.Id)),
                orderBy: orderBy,
                skip: (filterDto.PageNumber - 1) * filterDto.PageSize,
                take: filterDto.PageSize,
                asNoTracking: true
            );

            // 5. جلب الخصومات النشطة للحساب
            var now = DateTime.UtcNow;
            var activeDiscounts = await _discountRepository.GetAllAsync(
                filters: [d => d.StartDate <= now && d.EndDate >= now],
                includeChain: x => x.Include(i => i.DiscountCategories).Include(i => i.DiscountProducts)
            );

            var mappedData = products.Select(p => MapToProductResponse(p, activeDiscounts, lang.Id));

            return mappedData;
        }

        public async Task<int> GetCount(string language, ProductFilterDTO filterDto)
        {
            var lang = await _languageService.GetByCode(language);

            var filters = new List<Expression<Func<Product, bool>>>();

            if (filterDto.CategoryId.HasValue)
                filters.Add(p => p.CategoryId == filterDto.CategoryId.Value);

            if (filterDto.BrandId.HasValue)
                filters.Add(p => p.BrandId == filterDto.BrandId.Value);

            if (filterDto.IsOrganic.HasValue)
                filters.Add(p => p.IsOrganic == filterDto.IsOrganic.Value);

            if (!string.IsNullOrEmpty(filterDto.SearchText))
            {
                // البحث في الترجمة بناءً على اللغة الحالية
                filters.Add(p => p.ProductTranslations.Any(t =>
                    t.LanguageId == lang.Id && t.Name.Contains(filterDto.SearchText)));
            }

            // 2. تحديد الـ OrderBy
            Func<IQueryable<Product>, IOrderedQueryable<Product>>? orderBy = null;
            if (filterDto.SortedByPrice == true)
            {
                orderBy = filterDto.sortedByDesc == true
                    ? q => q.OrderByDescending(p => p.Price)
                    : q => q.OrderBy(p => p.Price);
            }

            // 3. حساب الإجمالي بناءً على الفلاتر (مهم جداً للـ Pagination)
            return await _repository.CountAsync(filters.ToArray());
        }

        private ProductResponse MapToProductResponse(Product p, IEnumerable<Discount> discounts, int langId)
        {
            // الحصول على الترجمة الصحيحة بناءً على اللغة المطلوبة
            var trans = p.ProductTranslations.FirstOrDefault(t => t.LanguageId == langId)
                        ?? p.ProductTranslations.FirstOrDefault()
                        ?? new ProductTranslation();

            // 1. تحديد الخصومات المطبقة على هذا المنتج بعينه أو على القسم الخاص به
            var applicableDiscounts = discounts.Where(d =>
                d.DiscountProducts.Any(dp => dp.ProductId == p.Id) ||
                d.DiscountCategories.Any(dc => dc.CategoryId == p.CategoryId)
            );

            decimal maxDiscountAmount = 0;
            bool isFlash = false;
            DateTime? offerEndDate = null;

            // 2. حساب أكبر قيمة خصم ممكنة (Best Deal)
            foreach (var d in applicableDiscounts)
            {
                // حساب قيمة الخصم الحالية سواء كان نسبة مئوية أو قيمة ثابتة
                decimal currentDiscountAmount = d.IsPercentage
                    ? (p.Price * d.DiscountValue) // لو مخزنة كـ 0.20 مثلاً
                    : d.DiscountValue;

                if (currentDiscountAmount > maxDiscountAmount)
                {
                    maxDiscountAmount = currentDiscountAmount;
                    isFlash = d.IsFlashSale;
                    offerEndDate = d.EndDate; // لتشغيل الـ Timer في الـ Front-end
                }
            }

            return new ProductResponse
            {
                Id = p.Id,
                Name = trans.Name,
                Description = trans.Description,
                Unit = trans.Unit,
                Price = p.Price,
                FinalPrice = p.Price - maxDiscountAmount,
                DiscountPercentage = p.Price > 0 ? (int)((maxDiscountAmount / p.Price) * 100) : 0,
                ImagePath = _urlService.GetFullUrl(p.ImagePath),
                IsOrganic = p.IsOrganic,
                StockQuantity = p.StockQuantity,
                IsFlashSale = isFlash,
                OfferEndDate = offerEndDate // التاريخ الذي ينتهي فيه أكبر خصم متاح
            };
        }

        public async Task<ProductWithTranslatedResponse> GetById(int Id)
        {
            var product = await _repository.GetOneAsync(
                includeChain: q => q.Include(p => p.ProductTranslations).ThenInclude(pt => pt.Language),
                filters: [x => x.Id == Id]
            );

            if (product == null)
                return new ProductWithTranslatedResponse();

            var result = new ProductWithTranslatedResponse
            {
                Id = product.Id,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                IsOrganic = product.IsOrganic,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                ImagePath = _urlService.GetFullUrl(product.ImagePath),
                Translations = product.ProductTranslations.Select(pt => new ProductTranslationDto
                {
                    LanguageCode = pt.Language.Code,
                    Name = pt.Name,
                    Description = pt.Description,
                    Unit = pt.Unit
                }).ToList()
            };

            return result;
        }

        public async Task<ProductResponse> GetByIdAndLanguage(int Id, string language)
        {
            var lang = await _languageService.GetByCode(language);

            // جلب المنتج مع ترجمة واحدة فقط للغة المطلوبة
            var product = await _repository.GetOneAsync(
                includeChain: q => q.Include(p => p.ProductTranslations.Where(pt => pt.LanguageId == lang.Id)),
                filters: [x => x.Id == Id]
            );

            if (product == null) return null!;

            // جلب الخصومات النشطة حالياً لحساب السعر
            var now = DateTime.UtcNow;
            var activeDiscounts = await _discountRepository.GetAllAsync(
                filters: [d => d.StartDate <= now && d.EndDate >= now],
                includeChain: x =>x.Include(i => i.DiscountCategories).Include(i => i.DiscountProducts)
            );

            // استخدام ميثود الـ Mapping اللي عملناها قبل كدة لتوحيد الـ Logic
            return MapToProductResponse(product, activeDiscounts, lang.Id);
        }
    }
}
