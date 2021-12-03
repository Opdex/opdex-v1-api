using FluentAssertions;
using Opdex.Platform.Common.Models;
using System;
using System.Numerics;
using Xunit;

namespace Opdex.Platform.Common.Tests.Models;

public class FixedDecimalTests
{
    [Fact]
    public void Zero_IsDefault_True()
    {
        FixedDecimal.Zero.Should().Be(default);
    }

    [Theory]
    [InlineData("10000000", "0")]
    [InlineData("0.43923482", "1002.329353")]
    public void Equals_DifferentValue_False(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA == valueB).Should().Be(false);
        (valueA != valueB).Should().Be(true);
        valueA.Equals(valueB).Should().Be(false);
        valueA.Equals((object)valueB).Should().Be(false);
        valueA.CompareTo(valueB).Should().NotBe(0);
        valueA.CompareTo((object)valueB).Should().NotBe(0);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public void Equals_SameValueAnyPrecision_True(int input)
    {
        // Arrange
        var value = input.ToString();
        var previous = FixedDecimal.Parse(value);
        value += ".";
        for (byte x = 1; x < byte.MaxValue; x++)
        {
            value += "0";
            var current = FixedDecimal.Parse(value);

            // Act
            // Assert
            (current == previous).Should().Be(true);
            (current != previous).Should().Be(false);
            current.Equals(previous).Should().Be(true);
            current.Equals((object)previous).Should().Be(true);
            current.CompareTo(previous).Should().Be(0);
            current.CompareTo((object)previous).Should().Be(0);
        }
    }

    [Theory]
    [InlineData("-444", "-444.0000")]
    [InlineData("0", "-444")]
    [InlineData("1002.329353", "0.43923482")]
    public void LessThan_False(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA < valueB).Should().Be(false);
        valueA.CompareTo(valueB).Should().NotBe(-1);
        valueA.CompareTo((object)valueB).Should().NotBe(-1);
    }

    [Theory]
    [InlineData("-444", "0")]
    [InlineData("0.43923482", "1002.329353")]
    public void LessThan_True(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA < valueB).Should().Be(true);
        valueA.CompareTo(valueB).Should().Be(-1);
        valueA.CompareTo((object)valueB).Should().Be(-1);
    }

    [Theory]
    [InlineData("-444", "-444.0000")]
    [InlineData("-444", "0")]
    [InlineData("0.43923482", "1002.329353")]
    public void GreaterThan_False(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA > valueB).Should().Be(false);
        valueA.CompareTo(valueB).Should().NotBe(1);
        valueA.CompareTo((object)valueB).Should().NotBe(1);
    }

    [Theory]
    [InlineData("0", "-444")]
    [InlineData("1002.329353", "0.43923482")]
    public void GreaterThan_True(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA > valueB).Should().Be(true);
        valueA.CompareTo(valueB).Should().Be(1);
        valueA.CompareTo((object)valueB).Should().Be(1);
    }

    [Theory]
    [InlineData("0", "-444")]
    [InlineData("1002.329353", "0.43923482")]
    public void LessThanOrEqualTo_False(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA <= valueB).Should().Be(false);
    }

    [Theory]
    [InlineData("-444", "-444.0000")]
    [InlineData("-444", "0")]
    [InlineData("0.43923482", "1002.329353")]
    public void LessThanOrEqualTo_True(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA <= valueB).Should().Be(true);
    }

    [Theory]
    [InlineData("-444", "0")]
    [InlineData("0.43923482", "1002.329353")]
    public void GreaterThanOrEqualTo_False(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA >= valueB).Should().Be(false);
    }

    [Theory]
    [InlineData("-444", "-444.0000")]
    [InlineData("0", "-444")]
    [InlineData("1002.329353", "0.43923482")]
    public void GreaterThanOrEqualTo_True(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        // Assert
        (valueA >= valueB).Should().Be(true);
    }

    [Fact]
    public void Parse_NegativeZero_CanParse()
    {
        // Arrange
        var input = "-0";

        // Act
        var value = FixedDecimal.Parse(input);
        var stringified = value.ToString();

        // Assert
        stringified.Should().Be("0");
    }

    [Fact]
    public void Parse_MoreThan255NumbersAfterDecimalPoint_ThrowArgumentException()
    {
        // Arrage
        // Act
        static void Act() => FixedDecimal.Parse("9.9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999");

        // Assert
        Assert.Throws<ArgumentException>("value", Act);
    }

    [Fact]
    public void TryParse_MoreThan255NumbersAfterDecimalPoint_ReturnFalse()
    {
        // Arrage
        // Act
        var success = FixedDecimal.TryParse("9.9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999",
                                            out var result);

        // Assert
        success.Should().Be(false);
        result.Should().Be(FixedDecimal.Zero);
    }

    [Theory]
    [InlineData("-1.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("0.0")]
    [InlineData("0.000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000")]
    [InlineData("9999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999.999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999999")]
    public void Parse_Store_AtPrecision(string input)
    {
        // Arrange
        // Act
        var value = FixedDecimal.Parse(input);
        var stringified = value.ToString();

        // Assert
        stringified.Should().Be(input);
    }

    [Theory]
    [InlineData("0.0000000000001", "0.00000")]
    [InlineData("-1", "1")]
    public void GetHashCode_DifferentValue_NotEqual(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        var hashCodeA = valueA.GetHashCode();
        var hashCodeB = valueB.GetHashCode();

        // Assert
        hashCodeA.Should().NotBe(hashCodeB);
    }

    [Theory]
    [InlineData("0", "0.00000")]
    [InlineData("99999.99999000000000000000000000000000000000000000", "99999.999990000")]
    public void GetHashCode_SameValueDifferentPrecision_Equal(string a, string b)
    {
        // Arrange
        var valueA = FixedDecimal.Parse(a);
        var valueB = FixedDecimal.Parse(b);

        // Act
        var hashCodeA = valueA.GetHashCode();
        var hashCodeB = valueB.GetHashCode();

        // Assert
        hashCodeA.Should().Be(hashCodeB);
    }

    [Fact]
    public void Resize_ReducePrecisionNoLossOfAccuracy_Expected()
    {
        // Arrange
        var value = new FixedDecimal(100_000_000_000_000, 9);

        // Act
        var result = value.Resize(8);

        // Assert
        result.ScaledValue.Should().Be(10_000_000_000_000);
    }

    [Fact]
    public void Resize_ReducePrecisionWithLossOfAccuracy_Expected()
    {
        // Arrange
        var value = new FixedDecimal(100_000_000_000_006, 9);

        // Act
        var result = value.Resize(8);

        // Assert
        result.ScaledValue.Should().Be(10_000_000_000_000);
    }

    [Fact]
    public void Resize_SamePrecision_ReturnThis()
    {
        // Arrange
        var value = new FixedDecimal(100_000_000_000_000, 9);

        // Act
        var result = value.Resize(9);

        // Assert
        result.Should().Be(value);
    }

    [Theory]
    [InlineData(-999_444_222_999_222_111, 0, 255)]
    [InlineData(0, 50, 51)]
    [InlineData(888_888_888_888, 20, 200)]
    public void Resize_HigherPrecision_ScaleUp(long underlying, byte currentPrecision, byte targetPrecision)
    {
        // Arrange
        var value = new FixedDecimal(underlying, currentPrecision);

        // Act
        var result = value.Resize(targetPrecision);

        // Assert
        result.ScaledValue.Should().Be(underlying * BigInteger.Pow(10, targetPrecision - currentPrecision));
        result.Precision.Should().Be(targetPrecision);
    }
}