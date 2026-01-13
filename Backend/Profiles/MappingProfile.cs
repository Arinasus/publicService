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
            
            // Service -> ServiceDto
            CreateMap<Service, ServiceDto>()
                .ForMember(dest => dest.ProviderName, 
                    opt => opt.MapFrom(src => src.Provider != null ? src.Provider.ProviderName : ""))
                .ForMember(dest => dest.ProviderID,
                    opt => opt.MapFrom(src => src.ProviderID));

            // Contract -> ContractDto
            CreateMap<Contract, ContractDto>()
                .ForMember(dest => dest.UserName, 
                    opt => opt.MapFrom(src => src.User != null 
                        ? $"{src.User.FirstName} {src.User.LastName}" 
                        : ""))
                .ForMember(dest => dest.AddressLine,
                    opt => opt.MapFrom(src => src.Address != null 
                        ? $"{src.Address.City}, {src.Address.Street}, {src.Address.House}" 
                        : ""))
                .ForMember(dest => dest.Services,
                    opt => opt.MapFrom(src => src.Services.Where(s => s != null).ToList()))
                .ForMember(dest => dest.ProviderName, 
                    opt => opt.MapFrom(src => src.Provider != null ? src.Provider.ProviderName : ""));
            // Invoice -> InvoiceDto
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.ContractNumber, 
                    opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ContractNumber : ""))
                .ForMember(dest => dest.AddressLine,
                    opt => opt.MapFrom(src => src.Contract != null && src.Contract.Address != null 
                        ? $"{src.Contract.Address.City}, {src.Contract.Address.Street}, {src.Contract.Address.House}"
                        : ""));
            
            CreateMap<Payment, PaymentDto>();
            
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.UserEmail, 
                    opt => opt.MapFrom(src => src.User != null ? src.User.Email : ""));
            
            // DTO -> Model (для POST/PUT)
            CreateMap<ContractCreateDto, Contract>()
                .ForMember(dest => dest.ContractID, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.Services, opt => opt.Ignore())
                .ForMember(dest => dest.Provider, opt => opt.Ignore());
        }
    }
}