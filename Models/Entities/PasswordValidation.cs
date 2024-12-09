﻿using Api.Models.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Entities
{
    [Table("PasswordValidations")]
    public class PasswordValidation : BaseEntity
    {
        public User User { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpirationDate { get; set; }
    }
}
