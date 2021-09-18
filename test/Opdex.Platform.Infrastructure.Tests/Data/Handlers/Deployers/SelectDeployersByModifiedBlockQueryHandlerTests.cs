using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using Opdex.Platform.Infrastructure.Data.Handlers.Deployers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Deployers
{
    public class SelectDeployersByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectDeployersByModifiedBlockQueryHandler _handler;

        public SelectDeployersByModifiedBlockQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectDeployersByModifiedBlockQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public void SelectDeployersByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new SelectDeployersByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task SelectDeployersByModifiedBlockQuery_ExecutesQuery()
        {
            // Arrange
            const ulong modifiedBlock = 10;
            var command = new SelectDeployersByModifiedBlockQuery(modifiedBlock);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<DeployerEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("ModifiedBlock = @ModifiedBlock"))), Times.Once);
        }

        [Fact]
        public async Task SelectDeployersByModifiedBlockQuery_Returns()
        {
            // Arrange
            const ulong modifiedBlock = 10;

            var entities = new List<DeployerEntity>
            {
                new DeployerEntity
                {
                    Id = 1,
                    Address = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm",
                    Owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN",
                    IsActive = true,
                    CreatedBlock = 3,
                    ModifiedBlock = 4
                }
            };

            var expectedResponse = new List<Deployer> { new Deployer(1, "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm", "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", true, 3, 4) };

            var command = new SelectDeployersByModifiedBlockQuery(modifiedBlock);

            _dbContext.Setup(callTo => callTo.ExecuteQueryAsync<DeployerEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(entities);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeEquivalentTo(expectedResponse);
        }
    }
}
