using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Order
{
    public class Order : BaseEntity, IEntity
    {
        public Order()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
            LastModifiedDate = DateTime.Now;
        }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryCity { get; set; }
        public string DeliveryState { get; set; }
        public string DeliveryZipCode { get; set; }
    }
}
