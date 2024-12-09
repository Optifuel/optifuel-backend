using Api.Models.Common;
using Api.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.CompanyDTO
{
    public class CompanySending
    {
        public string BusinessName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string VatNumber { get; set; } = null!;
        public Address Address { get; set; } = null!;

    }
}
