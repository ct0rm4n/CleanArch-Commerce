using Core.Entities.Domain.Address;
using Core.ViewModel.Address;
using Core.Wrappers;
using Data.Commands.Data.Repositories;
using Data.Interfaces;
using Service.Helpers;

namespace Service
{
    public partial class AddressService : IAddressService
    {
        private readonly AddressRepository _repository;

        public AddressService(AddressRepository repository)
        {
            _repository = repository;
        }    
        public Address? Add(Address address)
        {
            var isAdded = false;
            try
            {
                var inserted = _repository.AddReturn(address);
                isAdded = inserted.Item2;
                return inserted.Item1;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        public List<Address> GetAll()
        {
            List<Address> addresss = new List<Address>();
            try
            {
                addresss = _repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
            }

            return addresss;
        }

        public Address Get(int Id)
        {
            Address address = new Address();
            try
            {
                address = _repository.GetById(Id);
            }
            catch (Exception ex)
            {
            }

            return address;
        }

        public bool Update(Address address)
        {
            bool isUpdated = false;
            try
            {
                isUpdated = _repository.Update(address);
            }
            catch (Exception ex)
            {
            }

            return isUpdated;
        }

        public bool Delete(Address address)
        {
            bool isDeleted = false;
            try
            {                
                isDeleted = _repository.Delete(address);
            }
            catch (Exception ex)
            {
            }
            return isDeleted;
        }
        public PagedResponse<List<Address>> Search(PaginationFilter filter)
        {

            var validFilter = new PaginationFilter(filter.PageNumber, filter.PageSize);
            var data = new List<Address>();
            if (!string.IsNullOrEmpty(filter.SerachText))
            {
                data = (GetAll()).ToList().Where(f =>
                    f.Street.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)
                    || f.Street.Contains(filter.SerachText, StringComparison.CurrentCultureIgnoreCase)).ToList();
            }
            else
            {
                data = (GetAll()).ToList();
            }
            var pagedData = data
                .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
                .Take(validFilter.PageSize)
                .ToList();

            var totalRecords = data.Count();
            var pagedReponse = PaginationHelper.CreatePagedReponse<Address>(pagedData, validFilter, totalRecords, "", "https://localhost:7152/Address/getall");
            return (pagedReponse);
        }
        public IList<string> GetValidation(AddressVM viewModel)
        {
            return _repository.GetValidationMessage(viewModel);
        }

        public IEnumerable<Address> BulkInsertReturn(IEnumerable<Address> entitys)
        {
            return _repository.BulkInsertReturn(entitys);
        }

        public IEnumerable<States> GetStates()
        {
            return (_repository.GetStates().Result).AsEnumerable<States>();
        }
        public IEnumerable<City> GetCityByStates(int stateId)
        {
            return (_repository.GetCityByStates(stateId).Result).AsEnumerable<City>();
        }
        public States? GetStatesByUf(string uf)
        {
            return _repository.GetStatesByUf(uf).Result;
        }

        public City? GetCityByName(string name)
        {
            return _repository.GetCityByName(name).Result;
        }
    }
}