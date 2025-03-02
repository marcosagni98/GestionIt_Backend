using Application.Dtos.Auth.Requests;
using FluentValidation;


namespace Application.Helpers.Validators.Auth;

/// <summary>
/// Validator for the <see cref="ResetPasswordRequestDto"/>.
/// Ensures the email and password fields meet the validation requirements.
/// </summary>
public class ResetPasswordRequestDtoValidator : AbstractValidator<ResetPasswordRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordRequestDtoValidator"/> class.
    /// Defines validation rules for the ResetPasswordRequestDto.
    /// </summary>
    public ResetPasswordRequestDtoValidator()
    {
        // Validate Email
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The email is required.")
            .EmailAddress().WithMessage("The email is not valid.")
            .MaximumLength(250).WithMessage("The email cannot be longer than 250 characters.");

        // Validate Password
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("The password is required.")
            .MinimumLength(6).WithMessage("The password must be at least 6 characters long.")
            .MaximumLength(100).WithMessage("The password cannot be longer than 100 characters.");
    }
}
