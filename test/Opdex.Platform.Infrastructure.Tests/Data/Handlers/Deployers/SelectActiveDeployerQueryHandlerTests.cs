using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using Opdex.Platform.Infrastructure.Data.Handlers.Deployers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Deployers;

public class SelectActiveDeployerQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectActiveDeployerQueryHandler _handler;

    public SelectActiveDeployerQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectActiveDeployerQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Query_Limit1()
    {
        // Arrange
        var query = new SelectActiveDeployerQuery();

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<It.IsAnyType>(
            It.Is<DatabaseQuery>(q => q.Sql.EndsWith("LIMIT 1;"))), Times.Once);
    }

    [Fact]
    public async Task SelectActiveDeployer_Success()
    {
        // Arrange
        var deployer = new DeployerEntity
        {
            Id = 5,
            Address = new Address("PUv5zxmYE3wKkCgnnF31wo4VC7UNAVzXzH"),
            PendingOwner = Address.Empty,
            Owner = new Address("PX4x5WcA7MAPpHnYJoBEXD47TNMBxH9SvD"),
            IsActive = true,
            CreatedBlock = 5000,
            ModifiedBlock = 5000
        };

        _dbContext.Setup(db => db.ExecuteFindAsync<DeployerEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync(deployer);

        // Act
        var result = await _handler.Handle(new SelectActiveDeployerQuery(), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<DeployerEntity>(It.IsAny<DatabaseQuery>()), Times.Once);
        result.Id.Should().Be(deployer.Id);
        result.Address.Should().Be(deployer.Address);
        result.PendingOwner.Should().Be(deployer.PendingOwner);
        result.Owner.Should().Be(deployer.Owner);
        result.IsActive.Should().Be(deployer.IsActive);
        result.CreatedBlock.Should().Be(deployer.CreatedBlock);
        result.ModifiedBlock.Should().Be(deployer.ModifiedBlock);
    }

    [Fact]
    public async Task SelectActiveDeployer_NotFound_DoesNotThrowNotFoundException()
    {
        // Arrange
        var command = new SelectActiveDeployerQuery();

        _dbContext.Setup(db => db.ExecuteFindAsync<DeployerEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync((DeployerEntity)null);

        // Act
        // Assert
        await _handler.Invoking(h => h.Handle(command, CancellationToken.None))
            .Should()
            .NotThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task SelectActiveDeployer_ReturnsNull()
    {
        // Arrange
        var command = new SelectActiveDeployerQuery();

        _dbContext.Setup(db => db.ExecuteFindAsync<DeployerEntity>(It.IsAny<DatabaseQuery>()))
            .ReturnsAsync((DeployerEntity)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteFindAsync<DeployerEntity>(It.IsAny<DatabaseQuery>()), Times.Once);
        result.Should().BeNull();
    }
}
