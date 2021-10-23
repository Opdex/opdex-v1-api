using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using System;
using Xunit;

namespace Opdex.Platform.Common.Tests.Models
{
    public class Sha256Tests
    {
        [Fact]
        public void Equals_DifferentValue_False()
        {
            // Arrange
            var valueA = Sha256.Parse("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955");
            var valueB = Sha256.Parse("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff");

            // Act
            // Assert
            (valueA == valueB).Should().Be(false);
            (valueA != valueB).Should().Be(true);
            valueA.Equals(valueB).Should().Be(false);
            valueA.Equals((object)valueB).Should().Be(false);
            valueA.CompareTo(valueB).Should().NotBe(0);
            valueA.CompareTo((object)valueB).Should().NotBe(0);
            valueA.GetHashCode().Should().NotBe(valueB.GetHashCode());
        }

        [Fact]
        public void Equals_SameValue_True()
        {
            // Arrange
            var valueA = Sha256.Parse("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955");
            var valueB = Sha256.Parse("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955");

            // Act
            // Assert
            (valueA == valueB).Should().Be(true);
            (valueA != valueB).Should().Be(false);
            valueA.Equals(valueB).Should().Be(true);
            valueA.Equals((object)valueB).Should().Be(true);
            valueA.CompareTo(valueB).Should().Be(0);
            valueA.CompareTo((object)valueB).Should().Be(0);
            valueA.GetHashCode().Should().Be(valueB.GetHashCode());
        }

        [Theory]
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955", "2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955")]
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955", "0000000000000000000000000000000000000000000000000000000000000000")]
        public void LessThan_False(string a, string b)
        {
            var valueA = Sha256.Parse(a);
            var valueB = Sha256.Parse(b);

            valueA.CompareTo(valueB).Should().NotBe(-1);
            valueA.CompareTo((object)valueB).Should().NotBe(-1);
        }

        [Fact]
        public void LessThan_True()
        {
            // Arrange
            var valueA = Sha256.Parse("0000000000000000000000000000000000000000000000000000000000000000");
            var valueB = Sha256.Parse("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955");

            valueA.CompareTo(valueB).Should().Be(-1);
            valueA.CompareTo((object)valueB).Should().Be(-1);
        }

        [Theory]
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955", "2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955")]
        [InlineData("0000000000000000000000000000000000000000000000000000000000000000", "2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955")]
        public void GreaterThan_False(string a, string b)
        {
            // Arrange
            var valueA = Sha256.Parse(a);
            var valueB = Sha256.Parse(b);

            valueA.CompareTo(valueB).Should().NotBe(1);
            valueA.CompareTo((object)valueB).Should().NotBe(1);
        }

        [Fact]
        public void GreaterThan_True()
        {
            var valueA = Sha256.Parse("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955");
            var valueB = Sha256.Parse("0000000000000000000000000000000000000000000000000000000000000000");

            valueA.CompareTo(valueB).Should().Be(1);
            valueA.CompareTo((object)valueB).Should().Be(1);
        }

        [Fact]
        public void ToString_SmallHash_FullyPadded()
        {
            var value = new Sha256(0);
            value.ToString().Should().Be("0000000000000000000000000000000000000000000000000000000000000000");
        }

        [Fact]
        public void ToString_LowercaseHexadecimal()
        {
            var value = new Sha256(UInt256.Parse("20560503549839873554784830895214177457339290984553481410204484739048835041621"));
            value.ToString().Should().Be("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955");
        }

        [Fact]
        public void ToString_StartsWith8_NoSignIndicator()
        {
            var value = Sha256.Parse("88a1406f2fe56879a1bec42482a36d9584834b043aa47aea3790fbccf3b30d8d");
            value.ToString().Should().Be("88a1406f2fe56879a1bec42482a36d9584834b043aa47aea3790fbccf3b30d8d");
        }

        [Fact]
        public void Parse_Null_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            static void Act() => Sha256.Parse(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c95")] // 63 chars
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c9555")] // 65 chars
        public void Parse_LengthInvalid_ThrowArgumentException(string hexadecimal)
        {
            // Arrange
            // Act
            void Act() => Sha256.Parse(hexadecimal);

            // Assert
            Assert.Throws<ArgumentException>(Act);
        }

        [Theory]
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955")]
        [InlineData("2D74D5264870791DBE22E487DE6FC54F5F918530D84C9925244E261F5782C955")]
        public void Parse_HexadecimalString_ValueCorrect(string hexadecimal)
        {
            // Arrange
            // Act
            var hash = Sha256.Parse(hexadecimal);

            // Assert
            hash.Should().Be(new Sha256(UInt256.Parse("20560503549839873554784830895214177457339290984553481410204484739048835041621")));
        }

        [Fact]
        public void Parse_HexadecimalStringMinValue_ValueCorrect()
        {
            // Arrange
            var hexadecimal = "0000000000000000000000000000000000000000000000000000000000000000";

            // Act
            var hash = Sha256.Parse(hexadecimal);

            // Assert
            hash.Should().Be(new Sha256(UInt256.MinValue));
        }

        [Fact]
        public void Parse_HexadecimalStringMaxValue_ValueCorrect()
        {
            // Arrange
            var hexadecimal = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

            // Act
            var hash = Sha256.Parse(hexadecimal);

            // Assert
            hash.Should().Be(new Sha256(UInt256.MaxValue));
        }

        [Fact]
        public void TryParse_Null_ReturnFalse()
        {
            // Arrange
            // Act
            var canParse = Sha256.TryParse(null, out var hash);

            // Assert
            canParse.Should().Be(false);
            hash.Should().Be(default);
        }

        [Theory]
        [InlineData("")]
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c95")] // 63 chars
        [InlineData("2d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c9555")] // 65 chars
        public void TryParse_LengthInvalid_ReturnFalse(string hexadecimal)
        {
            // Arrange
            // Act
            var canParse = Sha256.TryParse(hexadecimal, out var hash);

            // Assert
            canParse.Should().Be(false);
            hash.Should().Be(default);
        }

        [Fact]
        public void TryParse_HexadecimalString_ReturnTrue()
        {
            // Arrange
            var hexadecimal = "8d74d5264870791dbe22e487de6fc54f5f918530d84c9925244e261f5782c955";

            // Act
            var canParse = Sha256.TryParse(hexadecimal, out var hash);

            // Assert
            canParse.Should().Be(true);
            hash.Should().Be(new Sha256(UInt256.Parse("63982537013833446838623950273472142902315535234168692925001078742016258656597")));
        }

        [Fact]
        public void TryParse_HexadecimalStringMinValue_ReturnTrue()
        {
            // Arrange
            var hexadecimal = "0000000000000000000000000000000000000000000000000000000000000000";

            // Act
            var canParse = Sha256.TryParse(hexadecimal, out var hash);

            // Assert
            canParse.Should().Be(true);
            hash.Should().Be(new Sha256(UInt256.MinValue));
        }

        [Fact]
        public void TryParse_HexadecimalStringMaxValue_ReturnTrue()
        {
            // Arrange
            var hexadecimal = "ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff";

            // Act
            var canParse = Sha256.TryParse(hexadecimal, out var hash);

            // Assert
            canParse.Should().Be(true);
            hash.Should().Be(new Sha256(UInt256.MaxValue));
        }
    }
}
