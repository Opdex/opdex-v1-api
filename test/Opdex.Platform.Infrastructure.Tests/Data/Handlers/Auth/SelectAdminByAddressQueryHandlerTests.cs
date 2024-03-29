using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using Opdex.Platform.Infrastructure.Data.Handlers.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Auth;

public class SelectAdminByAddressQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectAdminByAddressQueryHandler _handler;

    public SelectAdminByAddressQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectAdminByAddressQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectAdminByAddressQuery("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", false);

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void SelectAdminByAddressQuery_InvalidAddress_ThrowsArgumentNullException(string address)
    {
        // Arrange
        void Act() => new SelectAdminByAddressQuery(new Address(address));

        // Act
        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Address must not be empty.");
    }

    [Fact]
    public async Task SelectAdminByAddressQuery_Success()
    {
        // Arrange
        var expectedEntity = new AdminEntity { Id = 1, Address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };

        _dbContext.Setup(db => db.ExecuteFindAsync<AdminEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(expectedEntity);

        // Act
        var result = await _handler.Handle(new SelectAdminByAddressQuery(expectedEntity.Address), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<AdminEntity>(It.IsAny<DatabaseQuery>()), Times.Once);

        result.Id.Should().Be(expectedEntity.Id);
        result.Address.Should().Be(expectedEntity.Address);
    }

    [Fact]
    public async Task SelectAdminByAddressQuery_Throws_NotFoundException()
    {
        // Arrange
        Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        // Act
        // Assert
        await _handler.Invoking(h => h.Handle(new SelectAdminByAddressQuery(address), CancellationToken.None))
            .Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"{nameof(Admin)} not found.");
    }

    [Fact]
    public async Task SelectAdminByAddressQuery_ReturnsNull()
    {
        // Arrange
        Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        const bool findOrThrow = false;

        // Act
        var result = await _handler.Handle(new SelectAdminByAddressQuery(address, findOrThrow), CancellationToken.None);

        // Assert
        result.Should().BeNull();

        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<AdminEntity>(It.IsAny<DatabaseQuery>()), Times.Once);
    }
}
