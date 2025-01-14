using Core.Entities.Domain;
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
        private readonly UserRepository _userRepository;
        public CheckoutService(IUserService userService, ShoppingCartItemRepository shoppingCartItemRepository, UserRepository userRepository)
        {
            _userService = userService;
            _shoppingCartItemRepository = shoppingCartItemRepository;
            _userRepository = userRepository;
        }
        public ShoppingCartItem Add(ShoppingCartItem item)
        {
            var isAdded = false;
            try
            {
                var inserted = _shoppingCartItemRepository.AddOrUpdate(item);
                isAdded = inserted.Item2;
                return inserted.Item1;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<Tuple<ShoppingCartItem, Product>>? GetAllByCustomerId(int customerId)
        {
            var inserted = _shoppingCartItemRepository.GetAllByCustomerId(customerId);            
            return inserted;
        }

        public decimal GetTotalByCustomerId(int customerId)
        {
            var total = _shoppingCartItemRepository.GetTotalCartByCustomerId(customerId);
            return (decimal)total;
        }
    }
}
