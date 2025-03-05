using Application.Dtos.Auth.Requests;
using FluentValidation;

namespace Application.Helpers.Validators.Auth;

/// <summary>
/// Validator for the <see cref="RegisterRequestDto"/>.
/// Ensures the fields are valid according to specified requirements.
/// </summary>
public class RegisterRequestDtoValidator : AbstractValidator<RegisterRequestDto>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterRequestDtoValidator"/> class.
    /// Defines validation rules for the RegisterRequestDto.
    /// </summary>
    public RegisterRequestDtoValidator()
    {
        // Validate Name
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The name is required.")
            .MaximumLength(100).WithMessage("The name cannot be longer than 100 characters.");

        // Validate Email
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("The email is required.")
            .EmailAddress().WithMessage("The email is not valid.")
            .MaximumLength(250).WithMessage("The email cannot be longer than 250 characters.");

        // Validate Password
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("The password is required.")
            .MaximumLength(100).WithMessage("The password cannot be longer than 100 characters.");
    }
}
