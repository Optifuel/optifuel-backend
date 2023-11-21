using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations.Schema;
using ApiCos.Models.Common;

namespace ApiCos.Models.Entities
{
    [Table("Users")]
    public class User : BaseEntity
    {
        [Email(ErrorMessage = "The email address is not valid")]
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public DateOnly DateBirth { get; set; }
        public DrivingLicense DrivingLicense { get; set; } = null!;
        public Password PasswordEncrypted { get; set; } = new Password();
        public Verification? Verification { get; set; } = new Verification();
       
    }
}
