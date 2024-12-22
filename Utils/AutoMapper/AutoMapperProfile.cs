using Api.DTOs.Common;
using Api.DTOs.CompanyDTO;
using Api.DTOs.GasStationDTO;
using Api.DTOs.UserDTO;
using Api.DTOs.VehicleDTO;
using Api.Models.Common;
using Api.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Api.Utils.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRequest, User>();
            CreateMap<PasswordRequest, Password>();
            CreateMap<User, UserSending>().ForMember(dest => dest.BusinessName, opt=> opt.MapFrom(src=> src.Company.BusinessName));
            CreateMap<UserSending, User>();
            CreateMap<UserEdit, User>();
            CreateMap<UserEdit, UserSending>();

            CreateMap<CompanyRequest, Company>();
            CreateMap<Company, CompanySending>();
            
            CreateMap<VehicleRequest, Vehicle>();
            CreateMap<Vehicle, VehicleRequest>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src=> src.Company.BusinessName));
            CreateMap<Vehicle, VehicleSending>().ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src=> src.Company.BusinessName));

        }
    }
}
