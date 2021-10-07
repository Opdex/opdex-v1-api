using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens
{
    public class GetTokenByAddressFromFullNodeQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetTokenByAddressFromFullNodeQueryHandler _handler;

        public GetTokenByAddressFromFullNodeQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetTokenByAddressFromFullNodeQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveCirrusBestBlockReceiptQuery_Send()
        {
            // Arrange
            var request = new GetTokenByAddressFromFullNodeQuery("PXVmcSebfJwMY4HKm8TAiRLmi7fjYQgCwY");
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveCirrusBestBlockReceiptQuery>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_CallCirrusGetStandardTokenContractSummaryQuery_Send()
        {
            // Arrange
            var request = new GetTokenByAddressFromFullNodeQuery("PXVmcSebfJwMY4HKm8TAiRLmi7fjYQgCwY");
            var cancellationToken = new CancellationTokenSource().Token;

            ulong blockHeight = 1000;
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBestBlockReceiptQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new BlockReceipt("cc5800476021cbdf1c4066e40eeae6b3d64418a93afdda56a53ed0326dc15308", blockHeight, DateTime.Now,
                                                        DateTime.Now, "9e07f34cafc7146cbe713f919d00855c5f37842d03dfc24bb52b043cbee727b9",
                                                        "aba77002480d5dcf45036ac30f7691e9182d6cd4e85384fb11b116c6dfc6e8ad",
                                                        "b4128016762b7c7a37c6d817a2de3f94d8112565e08f3226977ad3d27f0b4af5", Enumerable.Empty<string>()));

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetStandardTokenContractSummaryQuery>(query => query.Token == request.Address
                                                                                                                   && query.BlockHeight == blockHeight
                                                                                                                   && query.IncludeBaseProperties
                                                                                                                   && query.IncludeBaseProperties), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_Response_Map()
        {
            // Arrange
            var request = new GetTokenByAddressFromFullNodeQuery("PXVmcSebfJwMY4HKm8TAiRLmi7fjYQgCwY");
            var cancellationToken = new CancellationTokenSource().Token;

            ulong blockHeight = 1000;
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveCirrusBestBlockReceiptQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new BlockReceipt("cc5800476021cbdf1c4066e40eeae6b3d64418a93afdda56a53ed0326dc15308", blockHeight, DateTime.Now,
                                                        DateTime.Now, "9e07f34cafc7146cbe713f919d00855c5f37842d03dfc24bb52b043cbee727b9",
                                                        "aba77002480d5dcf45036ac30f7691e9182d6cd4e85384fb11b116c6dfc6e8ad",
                                                        "b4128016762b7c7a37c6d817a2de3f94d8112565e08f3226977ad3d27f0b4af5", Enumerable.Empty<string>()));

            var tokenSummary = new StandardTokenContractSummary(blockHeight);
            tokenSummary.SetBaseProperties("Bitcoin (Wrapped)", "xBTC", 8);
            tokenSummary.SetTotalSupply(2100000000000000);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(tokenSummary);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            response.Address.Should().Be(request.Address);
            response.Decimals.Should().Be((int)tokenSummary.Decimals.Value);
            response.Name.Should().Be(tokenSummary.Name);
            response.Sats.Should().Be(tokenSummary.Sats.Value);
            response.Symbol.Should().Be(tokenSummary.Symbol);
            response.Summary.Should().Be(null);
            response.TotalSupply.Should().Be(tokenSummary.TotalSupply.Value);
        }
    }
}
