using FluentValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Account
{
    public class ChangePasswordDto
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(a => a.OldPassword).NotEmpty().WithMessage("Old Password can't be blank!");
            RuleFor(a => a.OldPassword).MaximumLength(150).WithMessage("Old Password length is not greater than 150!");

            RuleFor(a => a.NewPassword).NotEmpty().WithMessage("New Password can't be blank!");
            RuleFor(a => a.NewPassword).MaximumLength(150).WithMessage("New Password length is not greater than 150!");

            RuleFor(a => a.ConfirmPassword).NotEmpty().WithMessage("Confirm Password can't be blank!");
            RuleFor(a => a.ConfirmPassword).MaximumLength(150).WithMessage("Confirm Password length is not greater than 150!");
            RuleFor(a => a).Custom((a, context) =>
            {
                if (a.NewPassword != a.ConfirmPassword)
                {
                    context.AddFailure(nameof(a.NewPassword), "The password and confirmation password do not match!");
                }
            });
        }
    }
}