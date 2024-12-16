using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.User
{
    public class AuthUser: BaseEntity, IEntity
    {
        public AuthUser()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public int UserId { get; set; }

        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public bool isActive { get; set; }
    }
}
