using DataAnnotationsExtensions;
using System.ComponentModel.DataAnnotations.Schema;
using Api.Models.Common;

namespace Api.Models.Entities
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
        public ChangePassword? ChangePassword { get; set; } = new ChangePassword();
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }
}
