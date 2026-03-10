using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.IRepositories;
using ApplicationLayer.Interfaces.IServices;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
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
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _repository;
        private readonly ILanguageService _languageService;
        private readonly IUrlService _urlService;

        public PostService(IRepository<Post> repository, ILanguageService languageService, IUrlService urlService)
        {
            _repository = repository;
            _languageService = languageService;
            _urlService = urlService;
        }

        // 1. إضافة مقال جديد
        public async Task Add(CreatePostRequest request)
        {
            var translations = new List<PostTranslationDto>();

            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                translations = JsonSerializer.Deserialize<List<PostTranslationDto>>(request.TranslationsJson);
            }
            else
            {
                throw new ArgumentException("Translations are required.");
            }

            var postTranslations = new List<PostTranslation>();

            foreach (var t in translations)
            {
                var lang = await _languageService.GetByCode(t.LanguageCode);
                postTranslations.Add(new PostTranslation
                {
                    LanguageId = lang.Id,
                    Title = t.Title,
                    Content = t.Content
                });
            }

            string? imagePath = null;
            if (request.Image != null)
            {
                var uploadedImage = await FileHelper.UploadAndSaveFileAsync(request.Image!, "images/posts");
                imagePath = uploadedImage.RelativePath;
            }

            var post = new Post
            {
                AuthorName = request.AuthorName,
                ImagePath = imagePath,
                PostTranslations = postTranslations
            };

            await _repository.AddAsync(post);
            await _repository.SaveChangesAsync();
        }

        // 2. تعديل مقال موجود
        public async Task Edit(int postId, EditPostRequest request)
        {
            var post = await _repository.GetOneAsync(
                filters: [x => x.Id == postId],
                includeChain: q => q.Include(p => p.PostTranslations)
            );

            if (post == null)
                throw new KeyNotFoundException("Post not found.");

            if (!string.IsNullOrEmpty(request.TranslationsJson))
            {
                var translations = JsonSerializer.Deserialize<List<PostTranslationDto>>(request.TranslationsJson);
                if (translations == null || translations.Count == 0)
                    throw new ArgumentException("Translations are required.");

                // تنظيف الترجمات القديمة (اختياري حسب الـ Repository عندك) أو تحديثها
                post.PostTranslations.Clear();

                foreach (var t in translations)
                {
                    var lang = await _languageService.GetByCode(t.LanguageCode);
                    post.PostTranslations.Add(new PostTranslation
                    {
                        LanguageId = lang.Id,
                        Title = t.Title,
                        Content = t.Content
                    });
                }
            }

            if (request.Image != null)
            {
                if (!string.IsNullOrEmpty(post.ImagePath))
                {
                    FileHelper.DeleteFile(post.ImagePath);
                }

                var uploadedImage = await FileHelper.UploadAndSaveFileAsync(request.Image!, "images/posts");
                post.ImagePath = uploadedImage.RelativePath;
            }

            post.AuthorName = request.AuthorName;

            _repository.UpdateAsync(post);
            await _repository.SaveChangesAsync();
        }


        public async Task<IEnumerable<PostResponse>> GetAllByLanguage(string language, PostFilterDTO filter)
        {
            var lang = await _languageService.GetByCode(language);

            // 1. بناء الفلاتر (Filters)
            var filters = new List<Expression<Func<Post, bool>>>();

            // فلتر البحث (Search) في العنوان أو المحتوى باللغة المختارة
            if (!string.IsNullOrEmpty(filter.SearchText))
            {
                filters.Add(p => p.PostTranslations.Any(pt =>
                    pt.LanguageId == lang.Id &&
                    (pt.Title.Contains(filter.SearchText) || pt.Content.Contains(filter.SearchText))));
            }

            // 2. تحديد الترتيب (Sorting)
            // بفرض أن الـ Repository عندك بيدعم orderBy و orderDirection
            Func<IQueryable<Post>, IOrderedQueryable<Post>>? orderBy = null;
            if (filter.SortedByDesc.HasValue)
            {
                orderBy = filter.SortedByDesc.Value
                    ? q => q.OrderByDescending(p => p.Id) // أو p.CreatedAt لو موجودة
                    : q => q.OrderBy(p => p.Id);
            }

            // 3. جلب البيانات مع الـ Pagination
            var skip = (filter.PageNumber - 1) * filter.PageSize;

            var posts = await _repository.GetAllAsync(
                filters: filters,
                includeChain: q => q.Include(p => p.PostTranslations.Where(pt => pt.LanguageId == lang.Id)),
                orderBy: orderBy,
                skip: skip,
                take: filter.PageSize
            );

            // 4. عمل Mapping للـ Response
            return posts.Select(p => new PostResponse
            {
                Id = p.Id,
                AuthorName = p.AuthorName,
                Title = p.PostTranslations.FirstOrDefault()?.Title ?? "",
                Content = p.PostTranslations.FirstOrDefault()?.Content ?? "",
                ImagePath = _urlService.GetFullUrl(p.ImagePath)
            });
        }

        // 5. جلب المقال بالترجمات (للـ Admin Dashboard)
        public async Task<PostWithTranslatedResponse> GetById(int id)
        {
            var post = await _repository.GetOneAsync(
                includeChain: q => q.Include(p => p.PostTranslations).ThenInclude(pt => pt.Language),
                filters: [x => x.Id == id]
            );

            if (post == null) return new PostWithTranslatedResponse();

            return new PostWithTranslatedResponse
            {
                Id = post.Id,
                AuthorName = post.AuthorName,
                ImagePath = _urlService.GetFullUrl(post.ImagePath),
                Translations = post.PostTranslations.Select(pt => new PostTranslationDto
                {
                    LanguageCode = pt.Language.Code,
                    Title = pt.Title,
                    Content = pt.Content
                }).ToList()
            };
        }


        public async Task<int> GetCount(string language, PostFilterDTO filterDto)
        {
            var lang = await _languageService.GetByCode(language);

            var filters = new List<Expression<Func<Post, bool>>>();

         

            if (!string.IsNullOrEmpty(filterDto.SearchText))
            {
                // البحث في الترجمة بناءً على اللغة الحالية
                filters.Add(p => p.PostTranslations.Any(t =>
                    t.LanguageId == lang.Id && t.Title.Contains(filterDto.SearchText)));
            }

            // 3. حساب الإجمالي بناءً على الفلاتر (مهم جداً للـ Pagination)
            return await _repository.CountAsync(filters.ToArray());
        }

        public async Task<PostResponse> GetByIdAndLanguage(int Id, string language)
        {

            var lang = await _languageService.GetByCode(language);
            var post = await _repository.GetOneAsync(
                includeChain: q => q.Include(c => c.PostTranslations.Where(ct => ct.LanguageId == lang.Id)),
                filters: [x => x.Id == Id]
            );

            var result = new PostResponse
            {
                Id = post.Id,
                AuthorName = post.AuthorName,
                ImagePath = _urlService.GetFullUrl(post.ImagePath),

                Title = post.PostTranslations.First()?.Title ?? "",
                Content = post.PostTranslations.First()?.Content ?? "",

            };
            return result;
        }

    }
}
