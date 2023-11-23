namespace ApiCos.DTOs.UserDTO
{
    public class ChangePasswordRequest
    {
        public string email { get; set; } = null!;
        public string oldPassword { get; set; } = null!;
        public string newPassword { get; set; } = null!;
    }
}
