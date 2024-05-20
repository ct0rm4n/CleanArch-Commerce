using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain
{
    public class User : BaseEntity, IEntity
    {
        public User()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }

        //Relational Properties
        public ICollection<UserRole> UserRoles { get; set; }

    }
}
