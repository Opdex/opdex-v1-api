using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses
{
    public class SelectAddressAllowancesByOwnerWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContextMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly SelectAddressAllowancesByOwnerWithFilterQueryHandler _handler;

        public SelectAddressAllowancesByOwnerWithFilterQueryHandlerTests()
        {
            _dbContextMock = new Mock<IDbContext>();
            _mapperMock = new Mock<IMapper>();
            _handler = new SelectAddressAllowancesByOwnerWithFilterQueryHandler(_dbContextMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_HappyPath_ExecuteQuery()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var result = await _handler.Handle(new SelectAddressAllowancesByOwnerWithFilterQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"), cancellationToken);

            // Assert
            _dbContextMock.Verify(callTo => callTo.ExecuteQueryAsync<AddressAllowanceEntity>(
                It.Is<DatabaseQuery>(query => query.Token == cancellationToken)), Times.Once);
        }

        [Fact]
        public async Task Handle_HappyPath_MapResult()
        {
            // Arrange
            var entities = Enumerable.Empty<AddressAllowanceEntity>();
            _dbContextMock.Setup(callTo => callTo.ExecuteQueryAsync<AddressAllowanceEntity>(It.IsAny<DatabaseQuery>())).ReturnsAsync(entities);

            // Act
            var result = await _handler.Handle(new SelectAddressAllowancesByOwnerWithFilterQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"), CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<IEnumerable<AddressAllowance>>(entities), Times.Once);
        }

        [Fact]
        public async Task Handle_HappyPath_ReturnMappedResult()
        {
            // Arrange
            var allowances = new List<AddressAllowance>
            {
                new AddressAllowance(5, 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "10000000", 5, 50)
            };

            _dbContextMock.Setup(callTo => callTo.ExecuteQueryAsync<AddressAllowanceEntity>(It.IsAny<DatabaseQuery>()))
                          .ReturnsAsync(Enumerable.Empty<AddressAllowanceEntity>());
            _mapperMock.Setup(callTo => callTo.Map<IEnumerable<AddressAllowance>>(It.IsAny<object>())).Returns(allowances);

            // Act
            var result = await _handler.Handle(new SelectAddressAllowancesByOwnerWithFilterQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(allowances);
        }
    }
}
