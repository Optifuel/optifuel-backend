using ApiCos.Models.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiCos.Models.Entities
{
    [Table("Companies")]
    public class Company: BaseEntity
    {
        public string BusinessName { get; set; } = null!;

        [StringLength(11)]
        public string VatNumber { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Address Address { get; set; } = null!;
        public List<Vehicle> Vehicles { get; set; } =  new List<Vehicle>();
        public List<User> Users { get; set; } = new List<User>();
    }
}
