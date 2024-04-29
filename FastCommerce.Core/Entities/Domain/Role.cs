using FastCommerce.Core.Entities.Abstract;
using FastCommerce.Core.Entities.Enum;

namespace FastCommerce.Core.Entities.Domain
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
