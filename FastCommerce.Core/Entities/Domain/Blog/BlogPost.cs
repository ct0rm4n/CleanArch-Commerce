using FastCommerce.Core.Entities.Abstract;
using FastCommerce.Core.Entities.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastCommerce.Core.Entities.Domain.Blog
{
    public class BlogPost : BaseEntity, IEntity
    {
        public BlogPost()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public string Name { get; set; }
        public string FullDescription { get; set; }
        public string Url { get; set; }
    }
}