using Core.Entities.Abstract;
using Core.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Domain.User
{
    public class UserRole : BaseEntity, IEntity
    {

        public UserRole()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
