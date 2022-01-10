using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Transactions;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Transactions;

public class RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;

    private readonly RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandler _handler;

    public RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new RetrieveCirrusUnverifiedTransactionSenderByHashQueryHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_CallCirrusGetRawTransactionQuery_Send()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(GetValidRawTransaction());

        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetRawTransactionQuery>(
            q => q.TransactionHash == request.TransactionHash), cancellationToken), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_RawTransactionNull_ReturnEmpty()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((RawTransactionDto)null);

        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));

        // Act
        var address = await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetRawTransactionQuery>(
            q => q.TransactionHash == request.TransactionHash), cancellationToken), Times.Exactly(3));
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionOneAddress_ReturnAddress()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        var expectedAddress = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh");

        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetValidRawTransaction(expectedAddress));

        // Act
        var address = await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetRawTransactionQuery>(
            q => q.TransactionHash == request.TransactionHash), cancellationToken), Times.Exactly(1));
        address.Should().Be(expectedAddress);
    }

    private RawTransactionDto GetValidRawTransaction(Address expectedAddress = default)
    {
        if (expectedAddress == default) expectedAddress = new Address("tPXUEzDyZDrR8YzQ6LiAJWhVuAKB8RUjyt");

        return new RawTransactionDto()
        {
            Vout = new[]
            {
                new VOutDto() { ScriptPubKey = new ScriptPubKeyDto() { Addresses = new[] { expectedAddress } } }
            }
        };
    }
}
