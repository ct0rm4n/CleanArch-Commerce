using Core.Entities.Domain.Banner;
using Core.Entities.Domain.Checkout;
using Data.Commands.Data.Repositories;
using Data.Interfaces;
using Service.Interfaces;

namespace Service.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IUserService _userService;
        private readonly ShoppingCartItemRepository _shoppingCartItemRepository;
        public CheckoutService(IUserService userService, ShoppingCartItemRepository shoppingCartItemRepository)
        {
            _userService = userService;
            _shoppingCartItemRepository = shoppingCartItemRepository;
        }
        public ShoppingCartItem Add(ShoppingCartItem item)
        {
            var isAdded = false;
            try
            {
                var inserted = _shoppingCartItemRepository.AddReturn(item);
                isAdded = inserted.Item2;
                return inserted.Item1;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
    }
}
