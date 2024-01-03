using DAL.Interfaces;

using FluentValidation;

namespace BLL.DTOs.Account
{
    public class AccountRegistrationDto
    {

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Gender { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public int ConfigLanguageId { get; set; }
    }

    public class AccountRegistrationDtoValidator : AbstractValidator<AccountRegistrationDto>
    {

        public AccountRegistrationDtoValidator(IAccountRepository accountRepository)
        {
            RuleFor(m => m.FirstName).NotEmpty().WithMessage("First name can't be blank!");
            RuleFor(m => m.LastName).NotEmpty().WithMessage("Last name can't be blank!");
            RuleFor(m => m.Gender).NotEmpty().WithMessage("Gender can't be blank!");
            RuleFor(m => m.Email).NotEmpty().EmailAddress().WithMessage("Email is invalid!");
            RuleFor(m => m.ConfirmPassword).Equal(reg => reg.Password).WithMessage("Password does not match!");
            RuleFor(m => m.Phone).NotEmpty().WithMessage("Phone can't be blank!");
        }
    }
}
