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

        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetRawTransactionQuery>(
            q => q.TransactionHash == request.TransactionHash), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RawTransactionNull_ReturnEmpty()
    {
        // Arrange
        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync((RawTransactionDto)null);

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionVoutNull_ReturnEmpty()
    {
        // Arrange
        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RawTransactionDto()
            {
                Vout = null
            });

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionVoutEmpty_ReturnEmpty()
    {
        // Arrange
        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RawTransactionDto()
            {
                Vout = Array.Empty<VOutDto>()
            });

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionScriptPubKeyNull_ReturnEmpty()
    {
        // Arrange
        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RawTransactionDto()
            {
                Vout = new[]
                {
                    new VOutDto()
                    {
                        ScriptPubKey = null
                    }
                }
            });

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionAddressesNull_ReturnEmpty()
    {
        // Arrange
        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RawTransactionDto()
            {
                Vout = new[]
                {
                    new VOutDto()
                    {
                        ScriptPubKey = new ScriptPubKeyDto()
                        {
                            Addresses = null
                        }
                    }
                }
            });

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionAddressesEmpty_ReturnEmpty()
    {
        // Arrange
        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RawTransactionDto()
            {
                Vout = new[]
                {
                    new VOutDto()
                    {
                        ScriptPubKey = new ScriptPubKeyDto()
                        {
                            Addresses = Array.Empty<Address>()
                        }
                    }
                }
            });

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionMoreThanOneAddress_ReturnEmpty()
    {
        // Arrange
        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RawTransactionDto()
            {
                Vout = new[]
                {
                    new VOutDto()
                    {
                        ScriptPubKey = new ScriptPubKeyDto()
                        {
                            Addresses = new[]
                            {
                                new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh"),
                                new Address("PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi")
                            }
                        }
                    }
                }
            });

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(Address.Empty);
    }

    [Fact]
    public async Task Handle_RawTransactionOneAddress_ReturnAddress()
    {
        // Arrange
        var expectedAddress = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh");

        var request = new RetrieveCirrusUnverifiedTransactionSenderByHashQuery(new Sha256(235345245268));
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetRawTransactionQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RawTransactionDto()
            {
                Vout = new[]
                {
                    new VOutDto()
                    {
                        ScriptPubKey = new ScriptPubKeyDto()
                        {
                            Addresses = new[]
                            {
                                expectedAddress
                            }
                        }
                    }
                }
            });

        // Act
        var address = await _handler.Handle(request, CancellationToken.None);

        // Assert
        address.Should().Be(expectedAddress);
    }
}
