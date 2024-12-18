using Core.Entities.Domain.Checkout;


namespace Service.Interfaces
{
    public partial interface ICheckoutService
    {
        ShoppingCartItem Add(ShoppingCartItem item);
    }
}
