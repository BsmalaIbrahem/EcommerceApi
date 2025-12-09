using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Entities
{
    public class BaseTranslation : BaseEntity
    {
        public int Id { get; set; }
        public int LanguageId { get; set; }
        public Languages Language { get; set; } = null!;
        [NotMapped]
        public string LanguageCode => Language != null ? Language.Code : string.Empty;
    }
}
