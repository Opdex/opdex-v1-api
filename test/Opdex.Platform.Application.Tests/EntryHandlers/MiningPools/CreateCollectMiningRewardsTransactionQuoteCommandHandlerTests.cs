using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningPools
{
    public class CreateCollectMiningRewardsTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly CreateCollectMiningRewardsTransactionQuoteCommandHandler _handler;
        private readonly OpdexConfiguration _config;
        const string MethodName = MiningPoolConstants.Methods.CollectRewards;

        public CreateCollectMiningRewardsTransactionQuoteCommandHandlerTests()
        {
            _config = new OpdexConfiguration {ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions"};
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _handler = new CreateCollectMiningRewardsTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public void CreateCollectMiningRewardsTransactionQuoteCommand_InvalidMiningPool_ThrowArgumentNullException()
        {
            // Arrange
            Address miningPool = Address.Empty;
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";

            // Act
            void Act() => new CreateCollectMiningRewardsTransactionQuoteCommand(miningPool, walletAddress);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Mining pool address must be set.");
        }

        [Fact]
        public async Task CreateCollectMiningRewardsTransactionQuoteCommand_Sends_RetrieveMiningPoolByAddressQuery()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            var command = new CreateCollectMiningRewardsTransactionQuoteCommand(miningPool, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByAddressQuery>(c => c.Address == miningPool && c.FindOrThrow == true),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCollectMiningRewardsTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateCollectMiningRewardsTransactionQuoteCommand(miningPool, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                          && c.QuoteRequest.To == miningPool
                                                                                          && c.QuoteRequest.Amount == crsToSend
                                                                                          && c.QuoteRequest.Method == MethodName
                                                                                          && c.QuoteRequest.Callback != null
                                                                                          && c.QuoteRequest.Parameters.Count == 0),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateCollectMiningRewardsTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address miningPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateCollectMiningRewardsTransactionQuoteCommand(miningPool, walletAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(walletAddress, miningPool, crsToSend, MethodName, _config.WalletTransactionCallback);

            var expectedQuote = new TransactionQuote("1000", null, 23800, null, expectedRequest);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedQuote);

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(expectedQuote), Times.Once);
        }
    }
}
