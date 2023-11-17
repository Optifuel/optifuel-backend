using ApiCos.Models.Common;
using ApiCos.Models.Entities;

namespace ApiCos.DTOs.UserDTO
{
    public class UserSending
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public DateTime DateBirth { get; set; }
        public DrivingLicense DrivingLicense { get; set; } = null!;
        public string token { get; set; } = null!;

    }
}
