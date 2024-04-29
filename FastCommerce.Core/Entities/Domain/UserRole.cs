using FastCommerce.Core.Entities.Abstract;
using FastCommerce.Core.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastCommerce.Core.Entities.Domain
{
    public class UserRole : BaseEntity, IEntity
    {

        public UserRole()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
