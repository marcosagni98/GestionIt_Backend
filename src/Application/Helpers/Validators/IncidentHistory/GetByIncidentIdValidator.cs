using FluentValidation;

namespace Application.Validators;

/// <summary>
/// Validator for the GetByIncidentIdUseCase.
/// </summary>
public class GetByIncidentIdValidator : AbstractValidator<long>
{
    public GetByIncidentIdValidator()
    {
        RuleFor(incidentId => incidentId)
            .GreaterThan(0).WithMessage("Incident ID must be greater than 0.");
    }
}
