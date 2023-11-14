using ApiCos.DTOs.Common;
using ApiCos.DTOs.CompanyDTO;
using ApiCos.DTOs.GasStationDTO;
using ApiCos.DTOs.UserDTO;
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
            CreateMap<User, UserSending>();
            CreateMap<UserSending, User>();

            CreateMap<CompanyRequest, Company>();
            CreateMap<Company, CompanySending>();

        }
    }
}
