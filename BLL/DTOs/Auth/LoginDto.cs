using FluentValidation;

namespace BLL.DTOs.Auth
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LoginByPhoneDto
    {
        public string Phone { get; set; }
        public string Password { get; set; }
        public string DeviceToken { get; set; }
    }


    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(m => m.Username).NotEmpty().WithMessage("Username can't be blank!");
            RuleFor(m => m.Password).NotEmpty().WithMessage("Password can't be blank!");
        }
    }
    public class LoginByPhoneDtoValidator : AbstractValidator<LoginByPhoneDto>
    {
        public LoginByPhoneDtoValidator()
        {
            RuleFor(m => m.Phone).NotEmpty().WithMessage("Phone is not valid!");
            RuleFor(m => m.Password).NotEmpty().WithMessage("Password can't be blank!");
            RuleFor(m => m.DeviceToken).NotEmpty().WithMessage("DeviceToken can't be blank!");
        }
    }
}
