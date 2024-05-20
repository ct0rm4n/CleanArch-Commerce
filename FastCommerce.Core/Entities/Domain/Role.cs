using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain
{
    public class Role : BaseEntity, IEntity
    {

        public Role()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
    }
}
