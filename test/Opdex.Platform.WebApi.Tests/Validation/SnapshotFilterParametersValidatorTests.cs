using FluentValidation.TestHelper;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests;
using Opdex.Platform.WebApi.Validation;
using System;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation
{
    public class SnapshotFilterParametersValidatorTests
    {
        private readonly SnapshotFilterParametersValidator _validator;

        public SnapshotFilterParametersValidatorTests()
        {
            _validator = new SnapshotFilterParametersValidator();
        }

        [Fact]
        public void StartTime_GreaterThanEndTime_Invalid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(-1)
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.StartTime);
            result.ShouldHaveValidationErrorFor(request => request.EndTime);
        }

        [Fact]
        public void StartTime_SameAsEndTime_Invalid()
        {
            // Arrange
            var time = DateTime.UtcNow;
            var request = new SnapshotFilterParameters
            {
                StartTime = time,
                EndTime = time
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.StartTime);
            result.ShouldHaveValidationErrorFor(request => request.EndTime);
        }

        [Fact]
        public void StartTime_LessThanEndTime_Valid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                StartTime = DateTime.UtcNow.AddDays(-5),
                EndTime = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.StartTime);
            result.ShouldNotHaveValidationErrorFor(request => request.EndTime);
        }

        [Fact]
        public void Interval_OutsideValidRange_Invalid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                Interval = (Interval)9000
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Interval);
        }

        [Fact]
        public void Interval_GreaterThanTimeWindow_Invalid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                Interval = Interval.OneDay,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddHours(-12)
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Interval);
        }

        [Theory]
        [InlineData(Interval.OneHour)]
        [InlineData(Interval.OneDay)]
        public void Interval_Valid(Interval interval)
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                Interval = interval,
                StartTime = DateTime.UtcNow.AddMonths(-12),
                EndTime = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Interval);
        }

        [Fact]
        public void Limit_Invalid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                Limit = SnapshotCursor.MaxLimit + 1
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Limit);
        }

        [Fact]
        public void Limit_Valid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                Limit = SnapshotCursor.MaxLimit
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Limit);
        }
    }
}
