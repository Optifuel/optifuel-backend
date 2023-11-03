using FluentValidation;
using ApiCos.DTOs.UserDTO;
using ApiCos.Models.Common;

namespace ApiCos.Utils.Validators
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Surname).NotEmpty().MaximumLength(50);
            RuleFor(x => x.DateBirth).Must(x =>
            {
                var today = DateTime.Today;
                var age = today.Year - x.Year;

                if(x > today.AddYears(-age))
                {
                    age--;
                }

                return age >= 18;
            }).WithMessage("You must be at least 18 years old.");

            RuleFor(x => x.DrivingLicense).NotNull();
            RuleFor(x => x.DrivingLicense.Type)
                .NotEmpty()
                .IsEnumName(typeof(DrivingLicenseType), caseSensitive: false)
                .WithMessage("The driving license type is not valid.");
            RuleFor(x => x.Password)
                    .NotNull()
                    .MinimumLength(8)
                    .Matches(@"[A-Z]").WithMessage("The password must contain at least one capital letter.")
                    .Matches(@"[a-z]").WithMessage("The password must contain at least one tiny letter.")
                    .Matches(@"\d").WithMessage("The password must contain at least one number.")
                    .Matches(@"[^a-zA-Z0-9]").WithMessage("The password must contain at least a special character.");

            RuleFor(x=> x.BusinessName).NotEmpty().MaximumLength(50);   
        }

        private enum DrivingLicenseType
        {
            B1,
            B,
            C1,
            C,
            D1,
            D,
            BE,
            C1E,
            CE,
            D1E,
            DE
        }
    }
}
