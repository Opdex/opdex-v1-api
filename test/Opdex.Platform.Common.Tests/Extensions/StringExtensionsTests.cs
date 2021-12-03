using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using Xunit;

namespace Opdex.Platform.Common.Tests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData(null, false)]
    [InlineData("Test", true)]
    public void StringHasValue_Success(string value, bool expected)
    {
        value.HasValue().Should().Be(expected);
    }

    [Theory]
    [InlineData(" ", "", false)]
    [InlineData("  ", " ", false)]
    [InlineData("  ", null, false)]
    [InlineData("", "", true)]
    [InlineData(" ", " ", true)]
    [InlineData("test", "Test", true)]
    [InlineData("test", "monkey", false)]
    public void EqualsIgnoreCase(string current, string other, bool expected)
    {
        current.EqualsIgnoreCase(other).Should().Be(expected);
    }
}