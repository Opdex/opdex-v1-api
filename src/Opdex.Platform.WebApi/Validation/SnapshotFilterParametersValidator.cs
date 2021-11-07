using FluentValidation;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests;
using System;

namespace Opdex.Platform.WebApi.Validation
{
    public class SnapshotFilterParametersValidator : AbstractCursorValidator<SnapshotFilterParameters, SnapshotCursor>
    {
        public SnapshotFilterParametersValidator()
        {
            When(filter => filter.Cursor is null, () =>
            {
                RuleFor(filter => filter.StartTime).NotNull().LessThan(filter => filter.EndTime).WithMessage("Start time must be before end time.");
                RuleFor(filter => filter.EndTime).NotNull().GreaterThan(filter => filter.StartTime).WithMessage("End time must be after start time.");
                RuleFor(filter => filter.Interval).MustBeValidEnumValue().DependentRules(() =>
                {
                    RuleFor(filter => filter.Interval).Must((filter, interval) =>
                        {
                            var timeDifference = filter.EndTime.Subtract(filter.StartTime);
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
}
