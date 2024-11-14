using Application.Dtos.Auth.Requests;
using FluentValidation;


namespace Application.Validators.Auth;

/// <summary>
/// Validator for the ForgotPasswordRequestDto.
/// Ensures the email field is valid according to specified rules.
/// </summary>
public class ForgotPasswordRequestDtoValidator : AbstractValidator<ForgotPasswordRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForgotPasswordRequestDtoValidator"/> class.
    /// Defines validation rules for the ForgotPasswordRequestDto.
    /// </summary>
    public ForgotPasswordRequestDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The email is required")
            .EmailAddress().WithMessage("The email is not valid")
            .MaximumLength(250).WithMessage("The email cannot be longer than 250 characters");
    }
}