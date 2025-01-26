using AutoMapper;
using Core.Entities.Domain.Address;
using Core.ViewModel.Address;

namespace Service.Factory
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, AddressVM>().ReverseMap();
            CreateMap<States, StatesVM>().ReverseMap();
            CreateMap<City, CityVM>().ReverseMap();
        }
    }
}
