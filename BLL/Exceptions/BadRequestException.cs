using FluentValidation.Results;

namespace BLL.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message)
        {

        }

        public BadRequestException(ValidationResult validationResult) : base("Invalid data!")
        {
            ValidationErrors = validationResult.ToDictionary();
        }

        public BadRequestException(string message, ValidationResult validationResult) : base(message)
        {
            ValidationErrors = validationResult.ToDictionary();
        }

        public IDictionary<string, string[]>? ValidationErrors { get; set; }
    }
}
