using FluentValidation;

namespace PwC.CarRental.Application.Commands.Services;

public class RegisterServiceCommandValidator : AbstractValidator<RegisterServiceCommand>
{
    public RegisterServiceCommandValidator()
    {
        RuleFor(x => x.CarId)
            .NotEmpty()
            .WithMessage("CarId is required.");

        RuleFor(x => x.Date)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("Service date must be a valid date.");
    }
}
