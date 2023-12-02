using ApiCos.DTOs.Common;
using ApiCos.DTOs.CompanyDTO;
using ApiCos.DTOs.GasStationDTO;
using ApiCos.DTOs.UserDTO;
using ApiCos.DTOs.VehicleDTO;
using ApiCos.Models.Common;
using ApiCos.Models.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ApiCos.Utils.AutoMapper
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
            CreateMap<Vehicle, VehicleSending>().ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src=> src.Company.BusinessName));

        }
    }
}
