using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Account
{
    public class AccountUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? NationalCode { get; set; }
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Image { get; set; }
    }

    public class AccountOpUpdateDtoValidator : AbstractValidator<AccountUpdateDto>
    {
        public AccountOpUpdateDtoValidator()
        {
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("First Name can't be blank!");
            RuleFor(a => a.FirstName).MaximumLength(50).WithMessage("First Name length is not greater than 50!");

            RuleFor(a => a.LastName).NotEmpty().WithMessage("Lase Name can't be blank!");
            RuleFor(a => a.LastName).MaximumLength(50).WithMessage("Last Name length is not greater than 50!");

            RuleFor(a => a.Phone).NotEmpty().WithMessage("Phone can't be blank!").MaximumLength(10).WithMessage("Phone number is not greater than 10!");
            RuleFor(a => a.Phone).Matches(@"^(03[2-9]|05[689]|07[0-9]|08[1-9]|09[0-9])\d{7}$").WithMessage("Invalid phone number!");

            RuleFor(a => a.Address).MaximumLength(300).WithMessage("Address length is not greater than 300!");

            RuleFor(a => a.NationalCode).MaximumLength(150).WithMessage("Nationality code length is not greater than 10!");

            RuleFor(a => a.Gender).MaximumLength(10).WithMessage("Gender length is not greater than 10!");
        }
    }
}
