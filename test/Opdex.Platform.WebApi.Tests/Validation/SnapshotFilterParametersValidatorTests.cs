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
        public void StartDateTime_GreaterThanEndDateTime_Invalid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddDays(-1)
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.StartDateTime);
            result.ShouldHaveValidationErrorFor(request => request.EndDateTime);
        }

        [Fact]
        public void StartDateTime_SameAsEndDateTime_Invalid()
        {
            // Arrange
            var time = DateTime.UtcNow;
            var request = new SnapshotFilterParameters
            {
                StartDateTime = time,
                EndDateTime = time
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.StartDateTime);
            result.ShouldHaveValidationErrorFor(request => request.EndDateTime);
        }

        [Fact]
        public void StartDateTime_LessThanEndDateTime_Valid()
        {
            // Arrange
            var request = new SnapshotFilterParameters
            {
                StartDateTime = DateTime.UtcNow.AddDays(-5),
                EndDateTime = DateTime.UtcNow
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.StartDateTime);
            result.ShouldNotHaveValidationErrorFor(request => request.EndDateTime);
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
                StartDateTime = DateTime.UtcNow,
                EndDateTime = DateTime.UtcNow.AddHours(-12)
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
                StartDateTime = DateTime.UtcNow.AddMonths(-12),
                EndDateTime = DateTime.UtcNow
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
