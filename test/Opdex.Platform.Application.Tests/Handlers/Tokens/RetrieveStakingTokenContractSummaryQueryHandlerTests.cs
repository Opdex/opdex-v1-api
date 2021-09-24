using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Tokens
{
    public class RetrieveStakingTokenContractSummaryQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly  RetrieveStakingTokenContractSummaryQueryHandler _handler;

        public  RetrieveStakingTokenContractSummaryQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new  RetrieveStakingTokenContractSummaryQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveStakingTokenContractSummaryQuery_InvalidToken_ThrowsArgumentNullException()
        {
            // Arrange
            Address tokenAddress = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
        }

        [Fact]
        public void RetrieveStakingTokenContractSummaryQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 0;

            // Act
            void Act() => new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task RetrieveStakingTokenContractSummaryQuery_IncludeGenesis_Sends_CallCirrusGetSmartContractPropertyQuery()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight, includeGenesis: true), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.Contract == tokenAddress &&
                                                                                                       q.BlockHeight == blockHeight &&
                                                                                                       q.PropertyType == SmartContractParameterType.UInt64 &&
                                                                                                       q.PropertyStateKey == StakingTokenConstants.StateKeys.Genesis),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveStakingTokenContractSummaryQuery_IncludePeriodDuration_Sends_CallCirrusGetSmartContractPropertyQuery()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight, includePeriodDuration: true), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.Contract == tokenAddress &&
                                                                                                       q.BlockHeight == blockHeight &&
                                                                                                       q.PropertyType == SmartContractParameterType.UInt64 &&
                                                                                                       q.PropertyStateKey == StakingTokenConstants.StateKeys.PeriodDuration),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveStakingTokenContractSummaryQuery_IncludePeriodIndex_Sends_CallCirrusGetSmartContractPropertyQuery()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight, includePeriodIndex: true), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.Contract == tokenAddress &&
                                                                                                       q.BlockHeight == blockHeight &&
                                                                                                       q.PropertyType == SmartContractParameterType.UInt32 &&
                                                                                                       q.PropertyStateKey == StakingTokenConstants.StateKeys.PeriodIndex),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveStakingTokenContractSummaryQuery_IncludeVault_Sends_CallCirrusGetSmartContractPropertyQuery()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight, includeVault: true), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.Contract == tokenAddress &&
                                                                                                       q.BlockHeight == blockHeight &&
                                                                                                       q.PropertyType == SmartContractParameterType.Address &&
                                                                                                       q.PropertyStateKey == StakingTokenConstants.StateKeys.Vault),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveStakingTokenContractSummaryQuery_IncludeMiningGovernance_Sends_CallCirrusGetSmartContractPropertyQuery()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight, includeMiningGovernance: true), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.Contract == tokenAddress &&
                                                                                                       q.BlockHeight == blockHeight &&
                                                                                                       q.PropertyType == SmartContractParameterType.Address &&
                                                                                                       q.PropertyStateKey == StakingTokenConstants.StateKeys.MiningGovernance),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveStakingTokenContractSummaryQuery_Returns_StakingTokenContractSummary()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            Address miningGovernance = "P8q87nq2KGFTtoSPBDh2WUa8E7FiEUNG9X";
            Address vault = "P2WUa8E7BDhFiEUNG9X8q87nq2KGFTtoSP";
            const uint periodIndex = 1;
            const ulong periodDuration = 100;
            const ulong genesis = 50;

            _mediator.Setup(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.PropertyStateKey == StakingTokenConstants.StateKeys.MiningGovernance),
                                                   It.IsAny<CancellationToken>())).ReturnsAsync(new SmartContractMethodParameter(miningGovernance));

            _mediator.Setup(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.PropertyStateKey == StakingTokenConstants.StateKeys.Vault),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(new SmartContractMethodParameter(vault));

            _mediator.Setup(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.PropertyStateKey == StakingTokenConstants.StateKeys.PeriodDuration),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(new SmartContractMethodParameter(periodDuration));

            _mediator.Setup(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.PropertyStateKey == StakingTokenConstants.StateKeys.PeriodIndex),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(new SmartContractMethodParameter(periodIndex));

            _mediator.Setup(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.PropertyStateKey == StakingTokenConstants.StateKeys.Genesis),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(new SmartContractMethodParameter(genesis));

            // Act
            var response = await _handler.Handle(new RetrieveStakingTokenContractSummaryQuery(tokenAddress, blockHeight, true, true, true, true, true), CancellationToken.None);

            // Assert
            response.Genesis.Should().Be(genesis);
            response.PeriodDuration.Should().Be(periodDuration);
            response.PeriodIndex.Should().Be(periodIndex);
            response.Vault.Should().Be(vault);
            response.MiningGovernance.Should().Be(miningGovernance);
        }
    }
}
