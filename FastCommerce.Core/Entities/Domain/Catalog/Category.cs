using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Catalog
{
    public class Category : BaseEntity, IEntity
    {
        public Category()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }

        public string Name { get; set; }

        public string Html { get; set; }
        public string Seo { get; set; }
    }
}
