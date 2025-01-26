using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Address
{
    public class States : BaseEntity, IEntity
    {

        public States()
        {
            InsertedDate = DateTime.Now;
            LastModifiedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
        }

        public int UfId { get; set; }
        public string Name { get; set; }
        public string Uf { get; set; }
        public int Region { get; set; }
    }
}
