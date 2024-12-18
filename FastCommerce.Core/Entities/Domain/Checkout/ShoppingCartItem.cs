
using Core.Entities.Abstract;
using Core.Entities.Enum;

namespace Core.Entities.Domain.Checkout
{
    public class ShoppingCartItem : BaseEntity, IEntity
    {
        public ShoppingCartItem()
        {
            InsertedDate = DateTime.Now;
            Status = StatusEntity.Inserted;
            LastModifiedDate = DateTime.Now;
        }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
