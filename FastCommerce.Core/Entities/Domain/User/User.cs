using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.User
{
    public class User : BaseEntity, IEntity
    {
        public User()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
