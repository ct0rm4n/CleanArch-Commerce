using Core.Entities.Enum;
using Core.ViewModel.Generic.Abstracts;

namespace Core.ViewModel.User
{
    public class RoleVM : IBaseVM
    {
        public RoleVM()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Observation { get; set; } = string.Empty;
        public DateTime? InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public StatusEntity Status { get; set; }
    }
}
