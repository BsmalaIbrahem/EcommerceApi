using ApplicationLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Interfaces.IServices
{
    public interface IPostService
    {
        Task Add(CreatePostRequest request);
        Task Edit(int postId, EditPostRequest request);
        Task<IEnumerable<PostResponse>> GetAllByLanguage(string language, PostFilterDTO filter);
        Task<PostWithTranslatedResponse> GetById(int id);
        Task<int> GetCount(string language, PostFilterDTO filterDto);
        Task<PostResponse> GetByIdAndLanguage(int Id, string language);
    }
}
