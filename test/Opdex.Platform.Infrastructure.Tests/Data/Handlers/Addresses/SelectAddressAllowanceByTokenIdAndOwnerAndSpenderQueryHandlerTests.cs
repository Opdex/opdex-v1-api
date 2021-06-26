using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses
{
    public class SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler _handler;

        public SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task Handle_HappyPath_ExecuteFind()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var result = await _handler.Handle(
                new SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", false),
                cancellationToken);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteFindAsync<AddressAllowanceEntity>(
                It.Is<DatabaseQuery>(query => query.Token == cancellationToken)), Times.Once);
        }

        [Fact]
        public async Task Handle_NoResult_ReturnNull()
        {
            // Arrange
            _dbContext.Setup(callTo => callTo.ExecuteFindAsync<AddressAllowanceEntity>(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync((AddressAllowanceEntity)null);

            // Act
            var result = await _handler.Handle(
                new SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", false),
                default);

            // Assert
            result.Should().Be(null);
        }

        [Fact]
        public async Task Handle_FoundEntity_ReturnMapped()
        {
            // Arrange
            var entity = new AddressAllowanceEntity
            {
                Id = 5,
                TokenId = 10,
                LiquidityPoolId = 15,
                Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                Spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Allowance = "5000000000",
                CreatedBlock = 500,
                ModifiedBlock = 505
            };

            _dbContext.Setup(callTo => callTo.ExecuteFindAsync<AddressAllowanceEntity>(It.IsAny<DatabaseQuery>()))
                      .ReturnsAsync(entity);

            // Act
            var result = await _handler.Handle(
                new SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", false),
                default);

            // Assert
            result.Id.Should().Be(entity.Id);
            result.TokenId.Should().Be(entity.TokenId);
            result.LiquidityPoolId.Should().Be(entity.LiquidityPoolId);
            result.Owner.Should().Be(entity.Owner);
            result.Spender.Should().Be(entity.Spender);
            result.Allowance.Should().Be(entity.Allowance);
            result.CreatedBlock.Should().Be(entity.CreatedBlock);
            result.ModifiedBlock.Should().Be(entity.ModifiedBlock);
        }
    }
}
