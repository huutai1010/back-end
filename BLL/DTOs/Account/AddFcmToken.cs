using DAL.Interfaces;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTOs.Account
{
    public class AddFcmToken
    {
        public int AccountId { get; set; }
        public string Token { get; set; } = null!;
    }

    public class AddFcmTokenValidator : AbstractValidator<AddFcmToken>
    {

        public AddFcmTokenValidator(IAccountRepository accountRepository)
        {
            RuleFor(m => m.AccountId).NotEmpty().WithMessage("Account can't be blank!");
            RuleFor(m => m.Token).NotEmpty().WithMessage("Token can't be blank!");
        }
    }
}
