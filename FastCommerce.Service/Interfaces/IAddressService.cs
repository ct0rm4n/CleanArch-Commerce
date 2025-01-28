using Core.Entities.Domain.Address;
using Core.ViewModel.Address;
using Core.Wrappers;

namespace Data.Interfaces
{
    public partial interface IAddressService
    {
        Address? Add(Address blogPost);
        List<Address> GetAll();
        bool Update(Address user);
        bool Delete(Address user);
        Address Get(int Id);
        PagedResponse<List<Address>> Search(PaginationFilter filter);
        IList<string> GetValidation(AddressVM viewModel);
        IEnumerable<Address> BulkInsertReturn(IEnumerable<Address> entitys);
        IEnumerable<States> GetStates();
        IEnumerable<City> GetCityByStates(int stateId);
        States? GetStatesByUf(string uf);
        City? GetCityByName(string name);
    }
}
