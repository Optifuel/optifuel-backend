using Api.DTOs.Common;
using Api.Models.Common;

namespace Api.DTOs.UserDTO
{
    public class UserRequest: UserBase
    {
        public string BusinessName { get; set; } = null!;
        public DateOnly DateBirth { get; set; }
        public DrivingLicense DrivingLicense { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
