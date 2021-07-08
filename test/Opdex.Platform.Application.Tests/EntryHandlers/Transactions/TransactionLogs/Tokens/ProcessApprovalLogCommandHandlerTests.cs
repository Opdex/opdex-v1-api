using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessApprovalLogCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILogger<ProcessApprovalLogCommandHandler>> _loggerMock;
        private readonly ProcessApprovalLogCommandHandler _handler;

        public ProcessApprovalLogCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _loggerMock = new Mock<ILogger<ProcessApprovalLogCommandHandler>>();

            _handler = new ProcessApprovalLogCommandHandler(_mediatorMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_PersistTransactionLogCommand_Send()
        {
            // Arrange
            dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "9000000000", "50000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5),
                                                        "PR71udY85pAcNcitdDfzQevp6Zar9DizHM",
                                                        50000);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_CannotPersistTransactionLog_ReturnFalse()
        {
            // Arrange
            dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "9000000000", "50000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5),
                                                        "PR71udY85pAcNcitdDfzQevp6Zar9DizHM",
                                                        50000);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task Handle_RetrieveLiquidityPoolByAddress_Send()
        {
            // Arrange
            var contractAddress = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";

            dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "9000000000", "50000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, contractAddress, 5), "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 50000);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == contractAddress
                                                                                                   && !query.FindOrThrow),
                                                       CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_CannotFindLiquidityPool_ReturnFalse()
        {
            // Arrange
            dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "9000000000", "50000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5),
                                                        "PR71udY85pAcNcitdDfzQevp6Zar9DizHM",
                                                        50000);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((LiquidityPool)null);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().Be(false);
        }

        [Fact]
        public async Task Handle_RetrieveAddressAllowance_Send()
        {
            // Arrange
            var owner = "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi";
            var spender = "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7";
            var tokenId = 5L;

            dynamic log = SetupApprovalLog(owner, spender, "9000000000", "50000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PR71udY85pAcNcitdDfzQevp6Zar9DizHM", 5),
                                                        "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 50000);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Token(tokenId, "tokenAddress", false, "Opdex", "ODX", 8, 100_000_000, "1000000000", 99, 99));

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(
                query => query.TokenId == tokenId
                      && query.Owner == owner
                      && query.Spender == spender
                      && !query.FindOrThrow
            ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_UnableToRetrieveAddressAllowance_CreateAddressAllowance()
        {
            // Arrange
            var newAmount = "9000000000";
            var owner = "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi";
            var spender = "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7";
            var tokenId = 15L;
            var block = 50000UL;

            dynamic log = SetupApprovalLog(owner, spender, newAmount, oldAmount: "0");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PR71udY85pAcNcitdDfzQevp6Zar9DizHM", 5),
                                                        "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", block);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Token(tokenId, "tokenAddress", false, "Opdex", "ODX", 8, 100_000_000, "1000000000", 99, 99));
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync((AddressAllowance)null);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeAddressAllowanceCommand>(
                command => command.AddressAllowance.TokenId == tokenId
                        && command.AddressAllowance.Owner == owner
                        && command.AddressAllowance.Spender == spender
                        && command.AddressAllowance.Allowance == newAmount
                        && command.AddressAllowance.CreatedBlock == block
            ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_RetrievedAddressAllowanceButReprocessingSameOrOldBlock_DoNotUpdateAddressAllowancee()
        {
            // Arrange
            var newAmount = "9000000000";
            var addressAllowance = new AddressAllowance(5, 5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "8888888", 50, 500);

            dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", newAmount, "5000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PR71udY85pAcNcitdDfzQevp6Zar9DizHM", 5),
                                                        "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 500);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new LiquidityPool(5, "PRQ9AZ5ah3tdMqsRMtZKKLp6tQzJjFwPP9", 10, 15, 20, 5000, 5005));
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowance);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<MakeAddressAllowanceCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_RetrievedAddressAllowance_UpdateAddressAllowance()
        {
            // Arrange
            var newAmount = "9000000000";
            var addressAllowance = new AddressAllowance(5, 5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "8888888", 50, 500);

            dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", newAmount, "5000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PR71udY85pAcNcitdDfzQevp6Zar9DizHM", 5),
                                                        "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 501);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Token(5, "tokenAddress", false, "Opdex", "ODX", 8, 100_000_000, "1000000000", 99, 99));
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowance);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeAddressAllowanceCommand>(
                command => command.AddressAllowance == addressAllowance && addressAllowance.Allowance == newAmount
            ), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdatedAddressAllowance_ReturnTrue()
        {
            // Arrange
            var newAmount = "9000000000";
            var addressAllowance = new AddressAllowance(5, 5, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "8888888", 50, 500);

            dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", newAmount, "5000000");
            var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PR71udY85pAcNcitdDfzQevp6Zar9DizHM", 5),
                                                        "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 500000);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Token(5, "tokenAddress", false, "Opdex", "ODX", 8, 100_000_000, "1000000000", 99, 99));
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(addressAllowance);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().Be(true);
        }


        private ExpandoObject SetupApprovalLog(string owner, string spender, string newAmount, string oldAmount)
        {
            dynamic log = new ExpandoObject();
            log.owner = owner;
            log.spender = spender;
            log.amount = newAmount;
            log.oldAmount = oldAmount;
            return log;
        }
    }
}
