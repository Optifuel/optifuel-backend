using ApiCos.DTOs.Common;
using ApiCos.Models.Common;

namespace ApiCos.DTOs.UserDTO
{
    public class UserRequest: UserBase
    {
        public string BusinessName { get; set; } = null!;
        public DateTime DateBirth { get; set; }
        public DrivingLicense DrivingLicense { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
