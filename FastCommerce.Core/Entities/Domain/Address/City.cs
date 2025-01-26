
using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Address
{
    public class City: BaseEntity, IEntity
    {

        public City()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }

        public int CityId { get; set; }
        public string Name { get; set; }
        public int Uf { get; set; }
    }
}
