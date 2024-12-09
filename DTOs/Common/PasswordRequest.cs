namespace Api.DTOs.Common
{
    public class PasswordRequest
    {
        public string PasswordSalt { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
    }
}
