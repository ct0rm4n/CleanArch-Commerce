using FastCommerce.Core.Entities.Enum;

namespace FastCommerce.Core.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public int Id { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }

    }
}
