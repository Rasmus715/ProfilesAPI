using FluentValidation;

namespace AuthorizationAPI.ViewModels;

public class LoginViewModel
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsDoctor { get; set; } = false;
}

public class LoginValidator : AbstractValidator<LoginViewModel>
{
    public LoginValidator()
    {
        RuleFor(c => c.Email).EmailAddress().WithMessage("You've entered an invalid email"); ;
    }
}