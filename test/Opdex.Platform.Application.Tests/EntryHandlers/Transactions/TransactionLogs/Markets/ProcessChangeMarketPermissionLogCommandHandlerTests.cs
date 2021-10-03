using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Transactions.TransactionLogs.Markets
{
    public class ProcessChangeMarketPermissionLogCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ProcessChangeMarketPermissionLogCommandHandler _handler;

        public ProcessChangeMarketPermissionLogCommandHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new ProcessChangeMarketPermissionLogCommandHandler(_mediatorMock.Object,
                                                                          NullLogger<ProcessChangeMarketPermissionLogCommandHandler>.Instance);
        }

        [Fact]
        public async Task Handle_ExceptionThrown_ReturnFalse()
        {
            // Arrange
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>()))
                         .ThrowsAsync(new Exception("Something went wrong."));

            dynamic logData = SetupChangeMarketPermissionData("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", MarketPermissionType.Trade, true);
            var log = new ChangeMarketPermissionLog(logData, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5);

            // Act
            var response = await _handler.Handle(new ProcessChangeMarketPermissionLogCommand(log, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10_000),
                                                 default);

            // Assert
            response.Should().Be(false);
        }

        [Fact]
        public async Task Handle_TransactionLogNotPersisted_ReturnFalse()
        {
            // Arrange
            SetupRetrieveMarketCall();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(false);

            dynamic logData = SetupChangeMarketPermissionData("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", MarketPermissionType.Trade, true);
            var log = new ChangeMarketPermissionLog(logData, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5);

            // Act
            var response = await _handler.Handle(new ProcessChangeMarketPermissionLogCommand(log, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10_000),
                                                 default);

            // Assert
            response.Should().Be(false);
        }

        [Fact]
        public async Task Handle_MakeMarketPermissionCommand_Send()
        {
            // Arrange
            SetupRetrieveMarketCall();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            dynamic logData = SetupChangeMarketPermissionData("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", MarketPermissionType.Trade, true);
            var log = new ChangeMarketPermissionLog(logData, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5);
            var block = 10_000UL;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var response = await _handler.Handle(new ProcessChangeMarketPermissionLogCommand(log, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", block),
                                                 cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeMarketPermissionCommand>(
                command => command.MarketPermission.IsAuthorized == log.IsAuthorized
                        && command.MarketPermission.ModifiedBlock == block), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_Success_ReturnTrue()
        {
            // Arrange
            SetupRetrieveMarketCall();
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<PersistTransactionLogCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(true);

            dynamic logData = SetupChangeMarketPermissionData("PR71udY85pAcNcitdDfzQevp6Zar9DizHM", MarketPermissionType.Trade, true);
            var log = new ChangeMarketPermissionLog(logData, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 5);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeMarketPermissionCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(1ul);

            // Act
            var response = await _handler.Handle(new ProcessChangeMarketPermissionLogCommand(log, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10_000),
                                                 default);

            // Assert
            response.Should().Be(true);
        }

        private void SetupRetrieveMarketCall() =>
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMarketByAddressQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new Market(5,
                                                  "PR71udY85pAcNcitdDfzQevp6Zar9DizHM",
                                                  5,
                                                  5,
                                                  Address.Empty,
                                                  "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                  true,
                                                  true,
                                                  true,
                                                  3,
                                                  false,
                                                  500,
                                                  505));

        private ExpandoObject SetupChangeMarketPermissionData(Address address, MarketPermissionType permission, bool isAuthorized)
        {
            dynamic data = new ExpandoObject();
            data.address = address.ToString();
            data.permission = (byte)permission;
            data.isAuthorized = isAuthorized;
            return data;
        }
    }
}
