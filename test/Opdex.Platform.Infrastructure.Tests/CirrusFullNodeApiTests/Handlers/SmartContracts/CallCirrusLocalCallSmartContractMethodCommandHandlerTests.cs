using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.SmartContracts
{
    public class CallCirrusLocalCallSmartContractMethodCommandHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusLocalCallSmartContractMethodCommandHandler _handler;

        public CallCirrusLocalCallSmartContractMethodCommandHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();

            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _handler = new CallCirrusLocalCallSmartContractMethodCommandHandler(_smartContractsModuleMock.Object, mapper, new NullLoggerFactory());
        }

        [Fact]
        public void CallCirrusLocalCallSmartContractMethodCommand_InvalidQuoteRequest_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new CallCirrusLocalCallSmartContractMethodCommand(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Quote request must be provided.");
        }

        [Fact]
        public async Task CallCirrusLocalCallSmartContractMethodCommandHandler_Sends_LocalCallAsync()
        {
            // Arrange
            Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.1");
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            var parameters = new List<TransactionQuoteRequestParameter>()
            {
                new TransactionQuoteRequestParameter("Amount", UInt256.Parse("10"))
            };

            var cancellationToken = new CancellationTokenSource().Token;
            var request = new TransactionQuoteRequest(sender, to, amount, method, callback, parameters);

            // Act
            try
            {
                await _handler.Handle(new CallCirrusLocalCallSmartContractMethodCommand(request), cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(
                                                                                 requestDto => requestDto.ContractAddress == to
                                                                                            && requestDto.MethodName == method
                                                                                            && requestDto.Sender == sender
                                                                                            && requestDto.Amount == amount
                                                                                            && requestDto.Parameters.Length == parameters.Count
                                                                             ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CallCirrusLocalCallSmartContractMethodCommandHandler_Returns_TransactionQuote_Success()
        {
            // Arrange
            Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.1");
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            var parameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", UInt256.Parse("10"))
            };

            var request = new TransactionQuoteRequest(sender, to, amount, method, callback, parameters);

            dynamic txLog = new ExpandoObject();
            txLog.reserveCrs = 1ul;
            txLog.reserveSrc = "957488";

            var dtoResponse = new LocalCallResponseDto
            {
                ErrorMessage = new Error { Value = null },
                GasConsumed = new GasConsumed { Value = 10000 },
                Logs = new List<TransactionLogDto>
                {
                    new TransactionLogDto { Address = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", Data = "Data", Log = txLog, SortOrder = 0, Topics = new [] { "52657365727665734C6F67" }}
                }
            };

            _smartContractsModuleMock
                .Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dtoResponse);

            // Act
            var response = await _handler.Handle(new CallCirrusLocalCallSmartContractMethodCommand(request), CancellationToken.None);

            // Assert
            response.Error.Should().BeNull();
            response.Request.Should().BeEquivalentTo(request);
            response.Result.Should().BeNull();
            response.Logs.Count.Should().Be(1);
        }

        [Fact]
        public async Task CallCirrusLocalCallSmartContractMethodCommandHandler_Returns_TransactionQuote_Fail()
        {
            // Arrange
            Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
            Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
            FixedDecimal amount = FixedDecimal.Parse("1.1");
            const string method = "Swap";
            const string callback = "https://dev-api.opdex.com/transactions";

            var parameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Amount", UInt256.Parse("10"))
            };

            var request = new TransactionQuoteRequest(sender, to, amount, method, callback, parameters);

            const string error = "Error";

            var dtoResponse = new LocalCallResponseDto
            {
                ErrorMessage = new Error { Value = "Error" },
                GasConsumed = new GasConsumed { Value = 10000 },
                Logs = new List<TransactionLogDto>()
            };

            _smartContractsModuleMock
                .Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(dtoResponse);

            // Act
            var response = await _handler.Handle(new CallCirrusLocalCallSmartContractMethodCommand(request), CancellationToken.None);

            // Assert
            response.Error.Should().NotBe(error);
            response.Request.Should().BeEquivalentTo(request);
            response.Result.Should().BeNull();
            response.Logs.Should().BeEmpty();
        }
    }
}
