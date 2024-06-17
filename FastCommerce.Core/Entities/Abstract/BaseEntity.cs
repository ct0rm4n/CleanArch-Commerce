using Core.Entities.Enum;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        [Key]
        public int Id { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }

    }
}
