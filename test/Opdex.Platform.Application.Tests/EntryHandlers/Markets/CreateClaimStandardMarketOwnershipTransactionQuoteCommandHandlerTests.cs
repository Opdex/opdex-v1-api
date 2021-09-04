using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Markets;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets
{
    public class CreateClaimStandardMarketOwnershipTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly string _callbackEndpoint;
        private readonly CreateClaimStandardMarketOwnershipTransactionQuoteCommandHandler _handler;

        public CreateClaimStandardMarketOwnershipTransactionQuoteCommandHandlerTests()
        {
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _mediatorMock = new Mock<IMediator>();
            _callbackEndpoint = "https://dev-api.opdex.com/transactions";
            var configuration = new OpdexConfiguration();

            _handler = new CreateClaimStandardMarketOwnershipTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, configuration);
        }

        [Fact]
        public async Task Handle_RetrieveMarketByAddressQuery_FindOrThrow()
        {
            // Arrange
            Address market = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var request = new CreateClaimStandardMarketOwnershipTransactionQuoteCommand(market, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV");
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == market
                                                                                                 && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateClaimStandardMarketOwnershipTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address market = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address pendingOwner = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            const string crsToSend = "0";

            var command = new CreateClaimStandardMarketOwnershipTransactionQuoteCommand(market, pendingOwner);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == pendingOwner
                                                                                            && c.QuoteRequest.To == market
                                                                                            && c.QuoteRequest.Amount == crsToSend
                                                                                            && c.QuoteRequest.Method == StandardMarketConstants.Methods.ClaimPendingOwnership
                                                                                            && c.QuoteRequest.Callback != null
                                                                                            && c.QuoteRequest.Parameters.Count == 0),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateClaimStandardMarketOwnershipTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address market = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address pendingOwner = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            var command = new CreateClaimStandardMarketOwnershipTransactionQuoteCommand(market, pendingOwner);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(market, pendingOwner, "0", StandardMarketConstants.Methods.ClaimPendingOwnership, _callbackEndpoint);

            var expectedQuote = new TransactionQuote("PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjQf", null, 23800, null, expectedRequest);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedQuote);

            // Act
            try
            {
                await _handler.Handle(command, CancellationToken.None);
            }
            catch { }

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(expectedQuote), Times.Once);
        }
    }
}
