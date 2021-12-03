using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Allowances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Addresses.Allowances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Allowances;

public class GetAddressAllowanceQueryHandlerTests
{
    private readonly Mock<IModelAssembler<AddressAllowance, AddressAllowanceDto>> _assemblerMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetAddressAllowanceQueryHandler _handler;

    public GetAddressAllowanceQueryHandlerTests()
    {
        _assemblerMock = new Mock<IModelAssembler<AddressAllowance, AddressAllowanceDto>>();
        _mediatorMock = new Mock<IMediator>();
        _handler = new GetAddressAllowanceQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public void GetAddressAllowance_ThrowsArgumentNullException_InvalidOwner()
    {
        // Arrange
        const string token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const string spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address owner = Address.Empty;

        // Act
        void Act() => new GetAddressAllowanceQuery(owner, spender, token);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be provided.");
    }

    [Fact]
    public void GetAddressAllowance_ThrowsArgumentNullException_InvalidSpender()
    {
        // Arrange
        Address spender = Address.Empty;
        const string token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

        // Act
        void Act() => new GetAddressAllowanceQuery(owner, spender, token);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Spender must be provided.");
    }

    [Fact]
    public void GetAddressAllowance_ThrowsArgumentNullException_InvalidToken()
    {
        // Arrange
        const string spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address token = Address.Empty;

        // Act
        void Act() => new GetAddressAllowanceQuery(owner, spender, token);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token must be provided.");
    }

    [Fact]
    public async Task GetAddressAllowance_Sends_RetrieveAddressAllowanceQuery()
    {
        // Arrange
        var request = new GetAddressAllowanceQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                   "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                   "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK");

        var token = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(request, token);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressAllowanceQuery>(q => q.Owner == request.Owner &&
                                                                                             q.Spender == request.Spender &&
                                                                                             q.Token == request.Token),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAddressAllowance_Assembles_AddressAllowanceDto()
    {
        // Arrange
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const string spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        const string token = "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK";

        var request = new GetAddressAllowanceQuery(owner, spender, token);

        var cancellationToken = new CancellationTokenSource().Token;

        var allowance = new AddressAllowance(1, 2, owner, spender, new UInt256("1000000000"), 1, 1);

        _mediatorMock
            .Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceQuery>(), cancellationToken))
            .ReturnsAsync(allowance);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(allowance), Times.Once);
    }

    [Fact]
    public async Task GetAddressAllowance_Returns_AddressAllowanceDto()
    {
        // Arrange
        const string owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        const string spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        const string token = "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK";

        var request = new GetAddressAllowanceQuery(owner, spender, token);

        var cancellationToken = new CancellationTokenSource().Token;

        var allowance = new AddressAllowance(1, 2, owner, spender, new UInt256("1000000000"), 1, 1);
        var allowanceDto = new AddressAllowanceDto { Allowance = FixedDecimal.Parse("10.00000000"), Owner = owner, Spender = spender, Token = token };

        _mediatorMock
            .Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceQuery>(), cancellationToken))
            .ReturnsAsync(allowance);

        _assemblerMock
            .Setup(callTo => callTo.Assemble(allowance))
            .ReturnsAsync(allowanceDto);

        // Act
        var response = await _handler.Handle(request, cancellationToken);

        // Assert
        response.Should().Be(allowanceDto);
    }
}