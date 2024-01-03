
using DAL.Interfaces;
using FluentValidation;

namespace BLL.DTOs.Conversation
{
    public class AddConversationDto
    {
        public int AccountOneId { get; set; }
        public int AccountTwoId { get; set; }
    }
    public class AddConversationDtoValidator : AbstractValidator<AddConversationDto>
    {

        public AddConversationDtoValidator()
        {
            RuleFor(m => m.AccountOneId).NotEmpty().WithMessage("Account 1 can't be blank!");
            RuleFor(m => m.AccountTwoId).NotEmpty().WithMessage("Account 2 can't be blank!");
        }
    }
}
