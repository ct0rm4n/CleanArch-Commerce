using Core.Entities.Abstract;
using Core.Entities.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Domain.Blog
{
    public class BlogPost : BaseEntity, IEntity
    {
        public BlogPost()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public string Name { get; set; }
        public string Html { get; set; }
        public string Url { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int? UserId { get; set; }
        public bool Publish { get; set; }
        public bool Deleted { get; set; }
    }
}