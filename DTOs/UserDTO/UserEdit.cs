using Api.Models.Common;

namespace Api.DTOs.UserDTO
{
    public class UserEdit : UserBase
    {
        public string BusinessName { get; set; } = null!;
        public DateOnly DateBirth { get; set; }
        public DrivingLicense DrivingLicense { get; set; } = null!;
    }
}
