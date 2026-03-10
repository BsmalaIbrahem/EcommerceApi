using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class Post : BaseEntity
    {
        public int Id { get; set; }
        public string? ImagePath { get; set; }
        public string AuthorName { get; set; } = "Admin";
        public ICollection<PostTranslation> PostTranslations { get; set; } = new List<PostTranslation>();
    }

    public class PostTranslation : BaseTranslation
    {
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty; // محتوى المقالة
    }
}
