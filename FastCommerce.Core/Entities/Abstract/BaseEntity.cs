using Core.Entities.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace Core.Entities.Abstract
{
    public abstract class BaseEntity
    {
        public BaseEntity()
        {
            InsertedDate = DateTime.UtcNow;
            Status = StatusEntity.Inserted;
        }
        [Key]
        public int Id { get; set; }    
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }

    }
}
