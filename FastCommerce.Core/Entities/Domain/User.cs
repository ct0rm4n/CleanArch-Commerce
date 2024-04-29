using FastCommerce.Core.Entities.Abstract;
using FastCommerce.Core.Entities.Enum;

namespace FastCommerce.Core.Entities.Domain
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
