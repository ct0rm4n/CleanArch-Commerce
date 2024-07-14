using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Banner
{
    public class Banner : BaseEntity, IEntity
    {
        public Banner()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Binary { get; set; }
        public bool Publish { get; set; } = false;
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }
}