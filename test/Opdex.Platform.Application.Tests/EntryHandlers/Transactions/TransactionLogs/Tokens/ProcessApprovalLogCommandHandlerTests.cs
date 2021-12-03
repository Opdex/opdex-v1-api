using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions.TransactionLogs.Tokens;

public class ProcessApprovalLogCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ProcessApprovalLogCommandHandler>> _loggerMock;
    private readonly ProcessApprovalLogCommandHandler _handler;

    public ProcessApprovalLogCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ProcessApprovalLogCommandHandler>>();

        _handler = new ProcessApprovalLogCommandHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task ProcessApprovalLogCommand_Sends_RetrieveMarketRouterByAddressQuery()
    {
        // Arrange
        dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "9000000000", "50000000");
        var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5),
                                                    "PR71udY85pAcNcitdDfzQevp6Zar9DizHM",
                                                    50000);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveMarketRouterByAddressQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessApprovalLogCommand_Sends_RetrieveLiquidityPoolByAddressQuery()
    {
        // Arrange
        dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "9000000000", "50000000");
        var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5),
                                                    "PR71udY85pAcNcitdDfzQevp6Zar9DizHM",
                                                    50000);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ProcessApprovalLogCommand_Sends_RetrieveMiningPoolByAddressQuery()
    {
        // Arrange
        dynamic log = SetupApprovalLog("PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", "PJK7skDpACVauUvuqjBf7LXaWgRKCvMJL7", "9000000000", "50000000");
        var request = new ProcessApprovalLogCommand(new ApprovalLog(log, "PAdS3HnzJ5QhacRuQ5Yb5koAp4XxqswnXi", 5),
                                                    "PR71udY85pAcNcitdDfzQevp6Zar9DizHM",
                                                    50000);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>()), Times.Once);
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