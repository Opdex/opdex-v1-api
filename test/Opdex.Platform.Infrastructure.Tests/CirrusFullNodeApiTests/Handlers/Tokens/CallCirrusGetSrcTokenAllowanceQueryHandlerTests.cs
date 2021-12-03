using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Tokens;

public class CallCirrusGetSrcTokenAllowanceQueryHandlerTests
{
    private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
    private readonly CallCirrusGetSrcTokenAllowanceQueryHandler _handler;

    public CallCirrusGetSrcTokenAllowanceQueryHandlerTests()
    {
        _smartContractsModuleMock = new Mock<ISmartContractsModule>();

        _handler = new CallCirrusGetSrcTokenAllowanceQueryHandler(_smartContractsModuleMock.Object);
    }

    [Fact]
    public void CallCirrusGetSrcTokenAllowanceQuery_InvalidTokenAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address token = Address.Empty;
        Address owner = "PuJYUcmAsqAEgUXkBJPuCXfcNKdN28FQf5";
        Address spender = "PN28FQf5uJYUcmAsqAEgUXkBJPuCXfcNKd";

        // Act
        void Act() => new CallCirrusGetSrcTokenAllowanceQuery(token, owner, spender);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
    }

    [Fact]
    public void CallCirrusGetSrcTokenAllowanceQuery_InvalidOwnerAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address owner = Address.Empty;
        Address spender = "PN28FQf5uJYUcmAsqAEgUXkBJPuCXfcNKd";

        // Act
        void Act() => new CallCirrusGetSrcTokenAllowanceQuery(token, owner, spender);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Owner address must be provided.");
    }

    [Fact]
    public void CallCirrusGetSrcTokenAllowanceQuery_InvalidSpenderAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address owner = "PuJYUcmAsqAEgUXkBJPuCXfcNKdN28FQf5";
        Address spender = Address.Empty;

        // Act
        void Act() => new CallCirrusGetSrcTokenAllowanceQuery(token, owner, spender);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Spender address must be provided.");
    }

    [Fact]
    public async Task CallCirrusGetSrcTokenAllowanceQuery_Sends_LocalCallAsync()
    {
        // Arrange
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address owner = "PuJYUcmAsqAEgUXkBJPuCXfcNKdN28FQf5";
        Address spender = "PN28FQf5uJYUcmAsqAEgUXkBJPuCXfcNKd";
        const string methodName = StandardTokenConstants.Methods.Allowance;

        var parameters = new[] { new SmartContractMethodParameter(owner), new SmartContractMethodParameter(spender) };

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(new CallCirrusGetSrcTokenAllowanceQuery(token, owner, spender), cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.ContractAddress == token &&
                                                                                                         q.MethodName == methodName &&
                                                                                                         q.Parameters.All(p => parameters.Contains(p))
                                                                         ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetSrcTokenAllowanceQuery_Returns()
    {
        // Arrange
        Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
        Address spender = "PN28FQf5uJYUcmAsqAEgUXkBJPuCXfcNKd";
        UInt256 returnValue = UInt256.Parse("500000000");

        _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LocalCallResponseDto { Return = returnValue });

        // Act
        var response = await _handler.Handle(new CallCirrusGetSrcTokenAllowanceQuery(token, owner, spender), CancellationToken.None);

        // Assert
        response.Should().Be(returnValue);
    }
}