using Core.Entities.Domain.Address;
using Core.ViewModel.Address;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace Data.Commands.Data.Repositories
{
    public class AddressRepository : GenericRepository<Address, AddressVM>
    {
        private readonly string schema = "FastCommerce.dbo";
        public AddressRepository(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<IList<Core.Entities.Domain.Address.States>?> GetStates()
        {
            try
            {
                string tableName = GetTableName();
                string query = $"Select * from  {schema}.[State]";
                var connection = this.GetConnection();
                connection.Open();
                var query_result = (connection.Query<Core.Entities.Domain.Address.States>(query));
                return query_result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Core.Entities.Domain.Address.States?> GetStatesByUf(string uf)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"Select * from  {schema}.[State] where Uf = @UfAbreviation";
                var connection = this.GetConnection();
                connection.Open();
                var query_result = (connection.Query<Core.Entities.Domain.Address.States>(query, new { UfAbreviation = uf }));
                return query_result?.FirstOrDefault() ?? null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<IList<Core.Entities.Domain.Address.City>?> GetCityByStates(int UfId)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"Select * from  {schema}.[City] where Uf = {UfId}";
                var connection = this.GetConnection();
                connection.Open();
                var query_result = (connection.Query<Core.Entities.Domain.Address.City>(query));
                return query_result.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<City?> GetCityByName(string CityName)
        {
            try
            {
                string tableName = GetTableName();
                string query = $"Select * from  {schema}.[City] where Name = '{CityName}'";
                var connection = this.GetConnection();
                connection.Open();
                var query_result = (connection.Query<City>(query));
                return query_result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
