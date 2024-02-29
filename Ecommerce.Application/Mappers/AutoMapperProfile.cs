using AutoMapper;
using ECommerce.Application.Dtos.Authentication;
using ECommerce.Domain.Models.Application;

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
