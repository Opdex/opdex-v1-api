using FluentAssertions;
using Opdex.Platform.Common.Models;
using System;
using Xunit;

namespace Opdex.Platform.Common.Tests.Models.UInt
{
    public class AddressTests
    {
        [Theory]
        [InlineData("PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu")]
        [InlineData("0x14F768657135D3DaAFB45D242157055f1C9143f3")]
        public void CreatesNew_Address(string value)
        {
            // Arrange
            // Act
            var address = new Address(value);

            // Assert
            address.ToString().Should().Be(value);
        }

        [Theory]
        [InlineData("Wwi4mFP4Y1mhuNcRu")]
        [InlineData("0x14F768657135D3DaAFB45D242157055f1C9143f31")] // 1 extra
        public void CreatesNew_Address_ThrowsArgumentException(string value)
        {
            // Arrange
            // Act
            void Act() => new Address(value);

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Invalid address.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreatesNew_Address_Null(string value)
        {
            // Arrange
            // Act
            var address = new Address(value);

            // Assert
            address.ToString().Should().BeNull();
            address.Should().BeEquivalentTo(Address.Empty);
            (address == Address.Empty).Should().BeTrue();
        }

        [Fact]
        public void Address_Casts_AddressToString()
        {
            // Arrange
            const string value = "PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu";
            var address = new Address("PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu");

            // Act
            var cast = (string)address;

            // Assert
            cast.Should().Be(value);
        }

        [Fact]
        public void Address_Casts_StringToAddress()
        {
            // Arrange
            const string value = "PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu";
            var address = new Address("PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu");

            // Act
            var cast = (Address)value;

            // Assert
            cast.Should().Be(address);
        }

        [Fact]
        public void Address_ShouldMatch()
        {
            const string value = "PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu";
            Address address = new Address("PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu");

            address.Equals(address).Should().BeTrue();
            address.Equals(value).Should().BeTrue();
            (value == address).Should().BeTrue();
            (value != address).Should().BeFalse();
        }

        [Fact]
        public void Address_ShouldNotMatch()
        {
            const string value = "PPQdeXdjWDBzVLUjgWwi4mFP4Y1mhuNcRu";
            var address = new Address("zVLUjgWwi4mFUjgWwi4mFP4Y1mhuNcR3");

            (value != address).Should().BeTrue();
            (value == address).Should().BeFalse();
            address.Equals(value).Should().BeFalse();
        }
    }
}
