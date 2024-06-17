using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Commons
{
    public class Address : BaseEntity, IEntity
    {
        public Address()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public int CustomerAddressId { get; set; }
    }
}
