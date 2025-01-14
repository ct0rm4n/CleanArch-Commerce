using Core.Entities.Domain;
using Core.Entities.Domain.Checkout;

namespace Service.Interfaces
{
    public partial interface ICheckoutService
    {
        ShoppingCartItem Add(ShoppingCartItem item);
        List<Tuple<ShoppingCartItem, Product>>? GetAllByCustomerId(int customerId);
        decimal GetTotalByCustomerId(int customerId);
    }
}
