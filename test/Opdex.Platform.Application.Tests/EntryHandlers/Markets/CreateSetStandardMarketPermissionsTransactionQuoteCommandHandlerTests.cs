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
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets
{
    public class CreateSetStandardMarketPermissionsTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly OpdexConfiguration _config;
        private readonly CreateSetStandardMarketPermissionsTransactionQuoteCommandHandler _handler;

        public CreateSetStandardMarketPermissionsTransactionQuoteCommandHandlerTests()
        {
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _mediatorMock = new Mock<IMediator>();
            _config = new OpdexConfiguration {ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions"};
            _handler = new CreateSetStandardMarketPermissionsTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public async Task Handle_RetrieveMarketByAddressQuery_FindOrThrow()
        {
            // Arrange
            Address market = "PEkFDLUw1aLjYCWoJ1jRehNfTXjgWuZsX3";
            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(market, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV",
                                                                                        "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", Permissions.Provide, true);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(command, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMarketByAddressQuery>(query => query.Address == market
                                                                                                 && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateSetStandardMarketPermissionsTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address market = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address authority = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address user = "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK";
            Permissions permission = Permissions.Provide;
            bool authorize = true;
            FixedDecimal amount = 50;

            const string crsToSend = "0";

            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(market, authority, user, permission, authorize);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("User", new SmartContractMethodParameter(command.User)),
                new TransactionQuoteRequestParameter("Permission", new SmartContractMethodParameter((byte)command.Permission)),
                new TransactionQuoteRequestParameter("Authorize", new SmartContractMethodParameter(command.Authorize))
            };

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == authority
                                                                                            && c.QuoteRequest.To == market
                                                                                            && c.QuoteRequest.Amount == crsToSend
                                                                                            && c.QuoteRequest.Method == StandardMarketConstants.Methods.Authorize
                                                                                            && c.QuoteRequest.Callback != null
                                                                                            && c.QuoteRequest.Parameters.All(p => expectedParameters.Select(e => e.Value).Contains(p.Value))),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateSetStandardMarketPermissionsTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address market = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address authority = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            Address user = "PNEPCzpKSXns3jWtVfkF7WJeZKdNeEZTBK";
            Permissions permission = Permissions.Provide;
            bool authorize = true;
            FixedDecimal amount = 50;

            var command = new CreateSetStandardMarketPermissionsTransactionQuoteCommand(market, authority, user, permission, authorize);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(market, authority, "0", StandardMarketConstants.Methods.Authorize, _config.WalletTransactionCallback);

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
