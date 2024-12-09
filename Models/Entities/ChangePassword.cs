using System.ComponentModel.DataAnnotations;

namespace Api.Models.Entities
{
    public class ChangePassword
    {
        public int Token { get; set; }
        public DateTime DeadLine { get; set; }
        [Key]
        public int UserId { get; set; }
        public User User { get; set; }
        public byte[] NewPasswordSalt { get; set; } = new byte[32];
        public byte[] NewPasswordHash { get; set; } = new byte[32];
    }
}
