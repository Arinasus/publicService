using AutoMapper;
using Backend.Models;
using Backend.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Model -> DTO (для GET)
            CreateMap<User, UserDto>();
            CreateMap<Address, AddressDto>();
            CreateMap<Service, ServiceDto>() 
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider != null ? src.Provider.ProviderName : ""));
            CreateMap<Provider, ProviderDto>();
            CreateMap<Contract, ContractDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address.Street + ", " + src.Address.City))
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services))
                .ForMember(dest => dest.ProviderName, opt => opt.MapFrom(src => src.Provider.ProviderName));
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract.ContractNumber));
            CreateMap<Payment, PaymentDto>();
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User.Email));          
            CreateMap<ContractCreateDto, Contract>()
                .ForMember(dest => dest.ContractID, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Services, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore());
        }
    }
}