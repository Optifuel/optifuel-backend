using ApiCos.DTOs.Common;
using ApiCos.Models.Common;

namespace ApiCos.DTOs.UserDTO
{
    public class UserRequest
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string BusinessName { get; set; } = null!;
        public DateTime DateBirth { get; set; }
        public DrivingLicense DrivingLicense { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
