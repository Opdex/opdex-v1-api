using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests;
using System;

namespace Opdex.Platform.WebApi.Validation;

public class SnapshotFilterParametersValidator : AbstractCursorValidator<SnapshotFilterParameters, SnapshotCursor>
{
    public SnapshotFilterParametersValidator()
    {
        When(filter => filter.EncodedCursor is null, () =>
        {
            RuleFor(filter => filter.StartDateTime).NotNull().LessThan(filter => filter.EndDateTime).WithMessage("Start time must be before end time.");
            RuleFor(filter => filter.EndDateTime).NotNull().GreaterThan(filter => filter.StartDateTime).WithMessage("End time must be after start time.");
            RuleFor(filter => filter.Interval).MustBeValidEnumValue().DependentRules(() =>
            {
                RuleFor(filter => filter.Interval).Must((filter, interval) =>
                {
                    var timeDifference = filter.EndDateTime.Subtract(filter.StartDateTime);
                    return interval switch
                    {
                        Interval.OneHour => timeDifference.TotalHours >= 1,
                        Interval.OneDay => timeDifference.TotalDays >= 1,
                        _ => throw new ArgumentOutOfRangeException("Cannot validate the interval as it is outside handled range of values.")
                    };
                }).WithMessage("Interval cannot be larger than the given time frame.");
            });
        });

        RuleFor(filter => filter.Limit).LessThanOrEqualTo(SnapshotCursor.MaxLimit);
    }
}