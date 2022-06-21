using FluentValidation;
using WebAPI.DTO.User;

namespace WebAPI.Validations
{
    public class EmailSpecified : AbstractValidator<UserLoginDTO>
    {
        public EmailSpecified()
        {
            RuleFor(user => user.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("type your email")
                .EmailAddress().WithMessage("{PropertyName} use a valid email");
        }
    }

    public class PasswordSpecified : AbstractValidator<UserLoginDTO>
    {
        public PasswordSpecified()
        {
            RuleFor(user => user.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("type your password");
        }
    }


    public class UserLoginValidator : AbstractValidator<UserLoginDTO>
    {
        public UserLoginValidator()
        {
            Include(new EmailSpecified());
            Include(new PasswordSpecified());
        }
    }

}
