using ApiCos.Models.Common;
using ApiCos.Models.Entities;

namespace ApiCos.DTOs.UserDTO
{
    public class UserSending: UserBase
    {
        public DateOnly DateBirth { get; set; }
        public DrivingLicense DrivingLicense { get; set; } = null!;
        public string token { get; set; } = null!;
        public string BusinessName { get; set; } = null!;

    }
}
