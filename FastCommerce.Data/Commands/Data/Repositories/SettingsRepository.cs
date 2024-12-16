using Core.Entities.Abstract;
using Core.ViewModel.Generic.Abstracts;
using Data.Commands.Data;
using Microsoft.Extensions.Configuration;
namespace Data.Commands.Data.Repositories
{
    public class SettingsRepository : GenericRepository<Settings, SettingsVM>
    {
        public SettingsRepository(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
