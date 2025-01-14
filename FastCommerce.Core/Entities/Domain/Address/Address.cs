using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Address
{
    public class Address : BaseEntity, IEntity
    {

        public Address()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }
        public string ZipCode { get; set; }
        public string Street { get; set; }
        public string Neighborhood { get; set; }
        public string Complement { get; set; }
        public string Number { get; set; }
        public string City { get; set; }

        public bool Deleted { get; set; }
        public int? UserId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
    }
}
