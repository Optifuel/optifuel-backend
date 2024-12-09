using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }
       
    }
}
