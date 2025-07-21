using FluentValidation;

namespace PwC.CarRental.Application.Queries.Statistics;

public class GetMostRentedCarTypeUtilizationRankingQueryValidator : AbstractValidator<GetMostRentedCarTypeUtilizationRankingQuery>
{
    public GetMostRentedCarTypeUtilizationRankingQueryValidator()
    {
        RuleFor(x => x.StartDate)
            .LessThanOrEqualTo(x => x.EndDate)
            .WithMessage("StartDate must be before or equal to EndDate.");

        RuleFor(x => x.StartDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("StartDate must be a valid date.");

        RuleFor(x => x.EndDate)
            .GreaterThan(DateTime.MinValue)
            .WithMessage("EndDate must be a valid date.");

        RuleFor(x => x)
            .Must(x => (x.EndDate - x.StartDate).Days >= 0)
            .WithMessage("The period must be at least one day.");
    }
}
