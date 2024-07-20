using Core.Entities.Domain.User;
using Core.Entities.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ViewModel.User
{
    public class UserVM
    {

        public UserVM()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
