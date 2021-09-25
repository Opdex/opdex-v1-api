using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Permissions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Permissions
{
    public class PersistMarketPermissionCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistMarketPermissionCommandHandler _handler;

        public PersistMarketPermissionCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistMarketPermissionCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistMarketPermissionCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task Handle_CallDbContext_Persist()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        MarketPermissionType.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500);

            // Act
            var result = await _handler.Handle(new PersistMarketPermissionCommand(marketPermission), cancellationToken);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteScalarAsync<long>(
                It.Is<DatabaseQuery>(query => query.Token == cancellationToken)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_InsertSuccessfully_ReturnId()
        {
            // Arrange
            var id = 5L;
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        MarketPermissionType.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500);

            _dbContext.Setup(callTo => callTo.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(id);

            // Act
            var result = await _handler.Handle(new PersistMarketPermissionCommand(marketPermission), default);

            // Assert
            result.Should().Be(id);
        }

        [Fact]
        public async Task Handle_UpdateSuccessfully_ReturnId()
        {
            // Arrange
            var id = 10L;
            var marketPermission = new MarketPermission(id,
                                                        5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        MarketPermissionType.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500,
                                                        505);

            _dbContext.Setup(callTo => callTo.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(new PersistMarketPermissionCommand(marketPermission), default);

            // Assert
            result.Should().Be(id);
        }

        [Fact]
        public async Task Handle_ExceptionThrown_ReturnZero()
        {
            // Arrange
            var marketPermission = new MarketPermission(5,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                        MarketPermissionType.Trade,
                                                        true,
                                                        "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                        500);

            _dbContext.Setup(callTo => callTo.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                      .ThrowsAsync(new Exception("Something went wrong!"));

            // Act
            var result = await _handler.Handle(new PersistMarketPermissionCommand(marketPermission), default);

            // Assert
            result.Should().Be(0);
        }
    }
}
