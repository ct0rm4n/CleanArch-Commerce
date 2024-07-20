using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.User
{
    public class Role : BaseEntity, IEntity
    {
        public Role()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public string Name { get; set; }
        public string Observation { get; set; }
    }
}
