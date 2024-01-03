using BLL.DTOs.Place.PlaceCategory;
using BLL.DTOs.Place.PlaceDescription;
using BLL.DTOs.Place.PlaceImage;
using BLL.DTOs.Place.PlaceTime;
using BLL.DTOs.Place;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Staff
{
    public class CreateStaffDto
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; }
        public string? Image { get; set; }
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public int ConfigLanguageId { get; set; }
    }

    public class CreateStaffDtoValidator : AbstractValidator<CreateStaffDto>
    {
        public CreateStaffDtoValidator()
        {
            RuleFor(a => a.FirstName).NotEmpty().WithMessage("FirstName can't be blank!");
            RuleFor(a => a.FirstName).MaximumLength(50).WithMessage("FirstName length is not greater than 50!");

            RuleFor(a => a.Phone).NotEmpty().WithMessage("Phone can't be blank!");
            RuleFor(a => a.Phone).MaximumLength(20).WithMessage("Phone length is not greater than 20!");

            RuleFor(a => a.LastName).NotEmpty().WithMessage("LastName can't be blank!");
            RuleFor(a => a.LastName).MaximumLength(50).WithMessage("LastName length is not greater than 50!");

            RuleFor(a => a.Email).NotEmpty().WithMessage("Email can't be blank!");
            RuleFor(a => a.Email).EmailAddress().WithMessage("Email is wrong format!");

            RuleFor(a => a.Password).NotEmpty().WithMessage("Password can't be blank!");
            RuleFor(a => a.Password).MaximumLength(150).WithMessage("Password length is not greater than 150!");

            RuleFor(a => a.RoleId).NotEmpty().WithMessage("RoleId can't be blank!");
        }
    }
}
