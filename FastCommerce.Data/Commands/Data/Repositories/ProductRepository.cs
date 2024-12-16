using Core.Entities.Domain;
using Core.ViewModel.Catalog;
using Data.Commands.Data;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class ProductRepository : GenericRepository<Product, ProductVM>
    {
        public ProductRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
