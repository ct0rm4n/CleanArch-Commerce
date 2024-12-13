using Core.Entities.Abstract;
using Core.ViewModel.Generic.Abstracts;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Repositories
{
    public class SettingsRepository : GenericRepository<Settings, SettingsVM>
    {
        public SettingsRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
