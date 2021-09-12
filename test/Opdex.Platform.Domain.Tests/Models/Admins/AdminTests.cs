using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Admins;
using System;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Admins
{
    public class AdminTests
    {
        [Fact]
        public void CreateAdmin_InvalidId_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new Admin(0, new Address("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"));

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Id must be greater than 0.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateAdmin_InvalidAddress_ThrowsArgumentException(string address)
        {
            // Arrange
            // Act
            void Act() => new Admin(1, new Address(address));

            // Assert
            Assert.Throws<ArgumentException>(Act).Message.Should().Contain("Address must be provided.");
        }

        [Fact]
        public void CreateAdmin_Success()
        {
            // Arrange
            const long id = 1;
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            var admin = new Admin(id, address);

            // Assert
            admin.Id.Should().Be(id);
            admin.Address.Should().Be(address);
        }
    }
}
