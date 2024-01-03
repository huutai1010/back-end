using DAL.Interfaces;

using FluentValidation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Auth
{
    public class UpdateProfileDto : BaseDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; }
        public string? Image { get; set; }
        public string Phone { get; set; }
        public string NationalCode { get; set; }
        public string Address { get; set; }
        public string? Gender { get; set; }
        public int Status { get; set; }
    }

    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator(INationalityRepository nationalityRepository)
        {
            RuleFor(m => m.FirstName).NotEmpty().WithMessage("First name can't be blank!");
            RuleFor(m => m.LastName).NotEmpty().WithMessage("Last name can't be blank!");
            RuleFor(m => m.Gender).NotEmpty().WithMessage("Gender can't be blank!");
            RuleFor(m => m.Email).NotEmpty().EmailAddress().WithMessage("Email is invalid!");
            RuleFor(m => m.Phone).NotEmpty().WithMessage("Phone is invalid!");
            RuleFor(m => m.NationalCode).MustAsync((code, _) => nationalityRepository.NationCodeIsExist(code)).WithMessage("National code invalid!");
        }
    }
}
