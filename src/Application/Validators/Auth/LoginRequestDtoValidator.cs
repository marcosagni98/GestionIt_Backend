using Application.Dtos.Auth.Requests;
using FluentValidation;

namespace Application.Validators.Auth;

/// <summary>
/// Validator for the <see cref="LoginRequestDto"/>.
/// Ensures the email and password fields meet validation requirements.
/// </summary>
public class LoginRequestDtoValidator : AbstractValidator<LoginRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginRequestDtoValidator"/> class.
    /// Defines validation rules for the LoginRequestDto.
    /// </summary>
    public LoginRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The email is required.")
            .EmailAddress().WithMessage("The email is not valid.")
            .MaximumLength(250).WithMessage("The email cannot be longer than 250 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("The password is required.")
            .MaximumLength(100).WithMessage("The password cannot be longer than 100 characters.");
    }
}
