using FluentValidation;

namespace AuthorizationAPI.ViewModels;

public class RegisterViewModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
}

public class RegisterValidator : AbstractValidator<RegisterViewModel>
{
    public RegisterValidator()
    {
        RuleFor(c => c.Email).EmailAddress().WithMessage("You've entered an invalid email");
        RuleFor(c => c.Password).NotNull().WithMessage("Please, enter the password");
        RuleFor(c => c.ConfirmPassword).NotNull().WithMessage("Please, enter the confirmation password");
        RuleFor(r => r.Password).Equal(r => r.ConfirmPassword)
            .WithMessage("The passwords you’ve entered don’t coincide");
    }
}

