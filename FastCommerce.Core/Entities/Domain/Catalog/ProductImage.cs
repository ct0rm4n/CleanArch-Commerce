using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Catalog
{
    public class ProductImage : BaseEntity, IEntity
    {
        public ProductImage()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public int ProductId { get; set; }
        public string Binary { get; set; }
    }
}
