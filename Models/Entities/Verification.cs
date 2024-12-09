using System.ComponentModel.DataAnnotations;

namespace Api.Models.Entities
{
    public class Verification
    {
        public int Token { get; set; }
        public DateTime DeadLine { get; set; }
        [Key]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
