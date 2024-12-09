namespace Api.Models.Common
{
    public class Password
    {
        public byte[] PasswordSalt { get; set; } = new byte[32];
        public byte[] PasswordHash { get; set; } = new byte[32];
        public bool Validated { get; set; } = false;
    }
}
