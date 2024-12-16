using Core.Entities.Domain.Banner;
using Core.ViewModel.Banner;
using Data.Commands.Data;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class BannerRepository : GenericRepository<Banner, BannerVM>
    {
        public BannerRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
