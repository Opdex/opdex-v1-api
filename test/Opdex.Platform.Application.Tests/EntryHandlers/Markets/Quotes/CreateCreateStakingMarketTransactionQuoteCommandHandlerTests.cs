using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Markets.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Markets.Quotes
{
    public class CreateCreateStakingMarketTransactionQuoteCommandHandlerTests
    {
        private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly OpdexConfiguration _config;
        private readonly CreateCreateStakingMarketTransactionQuoteCommandHandler _handler;

        public CreateCreateStakingMarketTransactionQuoteCommandHandlerTests()
        {
            _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
            _mediatorMock = new Mock<IMediator>();
            _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
            _handler = new CreateCreateStakingMarketTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
        }

        [Fact]
        public async Task Handle_RetrieveActiveDeployerQuery_FindOrThrow()
        {
            // Arrange
            var command = new CreateCreateStakingMarketTransactionQuoteCommand("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy");
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveActiveDeployerQuery>(), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateCreateStakingMarketTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
        {
            // Arrange
            Address deployerOwner = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address stakingToken = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal crsToSend = FixedDecimal.Zero;

            var command = new CreateCreateStakingMarketTransactionQuoteCommand(deployerOwner, stakingToken);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Staking Token", new SmartContractMethodParameter(command.StakingToken))
            };

            var deployer = new Deployer(5, "PTotLfm9w7A4KBVq7sJgyP8Hd2MAU8vaRw", "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", true, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveDeployerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(deployer);

            // Act
            try
            {
                await _handler.Handle(command, cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == deployerOwner
                                                                                            && c.QuoteRequest.To == deployer.Address
                                                                                            && c.QuoteRequest.Amount == crsToSend
                                                                                            && c.QuoteRequest.Method == MarketDeployerConstants.Methods.CreateStakingMarket
                                                                                            && c.QuoteRequest.Callback != null
                                                                                            && c.QuoteRequest.Parameters.All(p => expectedParameters.Select(e => e.Value).Contains(p.Value))),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task CreateCreateStakingMarketTransactionQuoteCommand_Assembles_TransactionQuoteDto()
        {
            // Arrange
            Address deployerOwner = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address stakingToken = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";

            var command = new CreateCreateStakingMarketTransactionQuoteCommand(deployerOwner, stakingToken);
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedRequest = new TransactionQuoteRequest(deployerOwner, stakingToken, FixedDecimal.Zero, MarketDeployerConstants.Methods.CreateStakingMarket, _config.WalletTransactionCallback);

            var deployer = new Deployer(5, "PTotLfm9w7A4KBVq7sJgyP8Hd2MAU8vaRw", "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy", true, 500, 505);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveDeployerQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(deployer);

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
