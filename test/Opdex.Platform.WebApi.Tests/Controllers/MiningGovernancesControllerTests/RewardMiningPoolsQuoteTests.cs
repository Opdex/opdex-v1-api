using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MiningGovernancesControllerTests
{
    public class RewardMiningPoolsQuoteTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IApplicationContext> _contextMock;
        private readonly MiningGovernancesController _controller;

        public RewardMiningPoolsQuoteTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<IApplicationContext>();

            _controller = new MiningGovernancesController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
        }

        [Fact]
        public async Task RewardMiningPools_CreateRewardMiningPoolsTransactionQuoteCommand_Send()
        {
            // Arrange
            Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            Address miningGovernance = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
            var requestBody = new RewardMiningPoolsRequest { FullDistribution = true };
            var cancellationToken = new CancellationTokenSource().Token;

            _contextMock.Setup(get => get.Wallet).Returns(walletAddress);

            // Act
            await _controller.RewardMiningPools(miningGovernance, requestBody, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateRewardMiningPoolsTransactionQuoteCommand>(query => query.MiningGovernance == miningGovernance
                                                                                                       && query.WalletAddress == walletAddress), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task RewardMiningPools_CreateRewardMiningPoolsTransactionQuoteCommandResponse_Map()
        {
            // Arrange
            var dto = new TransactionQuoteDto();
            Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            Address miningGovernance = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
            var requestBody = new RewardMiningPoolsRequest { FullDistribution = true };
            var cancellationToken = new CancellationTokenSource().Token;

            _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateRewardMiningPoolsTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

            // Act
            await _controller.RewardMiningPools(miningGovernance, requestBody, cancellationToken);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TransactionQuoteResponseModel>(dto), Times.Once);
        }

        [Fact]
        public async Task RewardMiningPools_CreateRewardMiningPoolsTransactionQuoteCommandResponse_ReturnOk()
        {
            // Arrange
            var quoteResponse = new TransactionQuoteResponseModel();
            Address walletAddress = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
            Address miningGovernance = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";
            var requestBody = new RewardMiningPoolsRequest { FullDistribution = true };
            var cancellationToken = new CancellationTokenSource().Token;

            _contextMock.Setup(get => get.Wallet).Returns(walletAddress);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateRewardMiningPoolsTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TransactionQuoteDto());
            _mapperMock.Setup(callTo => callTo.Map<TransactionQuoteResponseModel>(It.IsAny<TransactionQuoteDto>())).Returns(quoteResponse);

            // Act
            var response = await _controller.RewardMiningPools(miningGovernance, requestBody, cancellationToken);

            // Act
            response.Should().BeOfType<OkObjectResult>();
            ((OkObjectResult)response).Value.Should().Be(quoteResponse);
        }
    }
}
