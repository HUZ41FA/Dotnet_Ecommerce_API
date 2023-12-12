using AutoMapper;
using ECommerce.Application.Dtos.Authentication;
using ECommerce.Domain.Application;

namespace ECommerce.Application.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        { 
            CreateMap<RegisterDto, SiteUser>();
            CreateMap<SiteUser, RegisterDto>();
        }
    }
}
