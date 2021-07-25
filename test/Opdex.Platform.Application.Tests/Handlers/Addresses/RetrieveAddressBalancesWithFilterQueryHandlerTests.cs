using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Handlers.Addresses;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses
{
    public class RetrieveAddressBalancesWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveAddressBalancesWithFilterQueryHandler _handler;

        public RetrieveAddressBalancesWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveAddressBalancesWithFilterQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public void RetrieveAddressBalancesWithFilter_ThrowsArgumentOutOfRangeException_InvalidSortDirection()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new RetrieveAddressBalancesWithFilterQuery(wallet, new string[0], false, false, (SortDirectionType)3, 10, 0, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Invalid sort direction");
        }

        [Fact]
        public void RetrieveAddressBalancesWithFilter_ThrowsArgumentOutOfRangeException_InvalidLimit()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new RetrieveAddressBalancesWithFilterQuery(wallet, new string[0], false, false, SortDirectionType.ASC, 0, 0, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Invalid limit");
        }

        [Fact]
        public async Task RetrieveAddressBalancesWithFilter_Sends_SelectAddressBalancesWithFilter()
        {
            // Arrange
            const string wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var tokens = new List<string> {"PAXxsvwuwGrJg1XnNCA8Swtpynzj2G8Eca"};
            const bool includeZeroBalances = true;
            const bool includeLpTokens = true;
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var request = new RetrieveAddressBalancesWithFilterQuery(wallet, tokens, includeLpTokens, includeZeroBalances,
                                                                     SortDirectionType.ASC, limit, 0, 0);

            // Act
            await _handler.Handle(request, It.IsAny<CancellationToken>());

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectAddressBalancesWithFilterQuery>(q =>
                                                                                                   q.Wallet == wallet &&
                                                                                                   q.Tokens.Any() &&
                                                                                                   q.Tokens == tokens &&
                                                                                                   q.Direction == direction &&
                                                                                                   q.Limit == limit &&
                                                                                                   q.IncludeZeroBalances == includeZeroBalances &&
                                                                                                   q.IncludeLpTokens == includeLpTokens),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
