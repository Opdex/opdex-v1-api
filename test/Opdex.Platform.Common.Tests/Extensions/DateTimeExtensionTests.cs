using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using System;
using System.Globalization;
using Xunit;
using Xunit.Abstractions;

namespace Opdex.Platform.Common.Tests.Extensions
{
    public class DateTimeExtensionTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DateTimeExtensionTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void RoundsToStartOfDay_Success()
        {
            var dateTime = new DateTime(2021, 6, 2, 5, 2, 1);

            var startOfDay = dateTime.StartOfDay();

            startOfDay.Hour.Should().Be(0);
            startOfDay.Minute.Should().Be(0);
            startOfDay.Second.Should().Be(0);
            startOfDay.Should().Be(new DateTime(2021, 6, 2));

            _testOutputHelper.WriteLine(dateTime.ToString(CultureInfo.InvariantCulture));
            _testOutputHelper.WriteLine(startOfDay.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void RoundToEndOfDay_Success()
        {
            var dateTime = new DateTime(2021, 6, 2, 5, 2, 1);

            var endOfDay = dateTime.EndOfDay();

            endOfDay.Day.Should().Be(dateTime.Day);
            endOfDay.Hour.Should().Be(23);
            endOfDay.Minute.Should().Be(59);
            endOfDay.Second.Should().Be(59);
            endOfDay.Should().Be(new DateTime(2021, 6, 2, 23, 59, 59));

            _testOutputHelper.WriteLine(dateTime.ToString(CultureInfo.InvariantCulture));
            _testOutputHelper.WriteLine(endOfDay.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void RoundsToStartOfHour_Success()
        {
            var dateTime = new DateTime(2021, 6, 2, 5, 2, 1);

            var startOfHour = dateTime.StartOfHour();

            startOfHour.Day.Should().Be(dateTime.Day);
            startOfHour.Hour.Should().Be(5);
            startOfHour.Minute.Should().Be(0);
            startOfHour.Second.Should().Be(0);
            startOfHour.Should().Be(new DateTime(2021, 6, 2, 5, 0, 0));

            _testOutputHelper.WriteLine(dateTime.ToString(CultureInfo.InvariantCulture));
            _testOutputHelper.WriteLine(startOfHour.ToString(CultureInfo.InvariantCulture));
        }

        [Fact]
        public void RoundToEndOfHour_Success()
        {
            var dateTime = new DateTime(2021, 6, 2, 5, 2, 1);

            var endOfHour = dateTime.EndOfHour();

            endOfHour.Day.Should().Be(dateTime.Day);
            endOfHour.Hour.Should().Be(5);
            endOfHour.Minute.Should().Be(59);
            endOfHour.Second.Should().Be(59);
            endOfHour.Should().Be(new DateTime(2021, 6, 2, 5, 59, 59));

            _testOutputHelper.WriteLine(dateTime.ToString(CultureInfo.InvariantCulture));
            _testOutputHelper.WriteLine(endOfHour.ToString(CultureInfo.InvariantCulture));
        }
    }
}
