using FluentValidation;
using Users.API.API;
using Users.API.Enums;

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).NotEmpty().Must(r => r == UserRole.Admin || r == UserRole.User);
    }
}
