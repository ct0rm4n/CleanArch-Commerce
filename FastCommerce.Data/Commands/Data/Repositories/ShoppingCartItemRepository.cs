using Core.Entities.Domain.Checkout;
using Core.ViewModel.Generic.Abstracts;
using Dapper;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class ShoppingCartItemRepository : GenericRepository<ShoppingCartItem, IBaseVM>
    {
        private readonly string schema = "FastCommerce.dbo";
        public ShoppingCartItemRepository(IConfiguration configuration) : base(configuration)
        {
        }
               
    }
}
