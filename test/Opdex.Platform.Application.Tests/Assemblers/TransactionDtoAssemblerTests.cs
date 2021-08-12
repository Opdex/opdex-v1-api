using AutoMapper;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers
{
    public class TransactionDtoAssemblerTests
    {
        private readonly Mock<IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>>> _eventsAssemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TransactionDtoAssembler _handler;

        public TransactionDtoAssemblerTests()
        {
            _eventsAssemblerMock = new Mock<IModelAssembler<IEnumerable<TransactionLog>, IReadOnlyCollection<TransactionEventDto>>>();
            _mediatorMock = new Mock<IMediator>();

            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();

            _handler = new TransactionDtoAssembler(_mediatorMock.Object, mapper, _eventsAssemblerMock.Object);
        }

        [Fact]
        public async Task AssembleTransactionDto_Sends_RetrieveTransactionLogsByTransactionIdQuery()
        {
            // Arrange
            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTransactionLogsByTransactionIdQuery>(q => q.TransactionId == transaction.Id),
                                                       It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task AssembleTransactionDto_Sends_RetrieveBlockByHeightQuery()
        {
            // Arrange
            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveBlockByHeightQuery>(q => q.Height == transaction.BlockHeight),
                                                       It.IsAny<CancellationToken>()), Times.Once());
        }

        [Fact]
        public async Task AssembleTransactionDto_Sends_TransactionEventsDtoAssembler()
        {
            // Arrange
            dynamic approval = new ExpandoObject();
            approval.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            approval.spender = "spender";
            approval.amount = "1";
            approval.oldAmount = "0";
            var approvalLog = new ApprovalLog(approval, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            dynamic transfer = new ExpandoObject();
            transfer.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            transfer.to = "to";
            transfer.amount = "32981";
            var transferLog = new TransferLog(transfer, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            dynamic swap = new ExpandoObject();
            swap.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            swap.to = "to";
            swap.amountCrsIn = 0ul;
            swap.amountSrcIn = "12";
            swap.amountCrsOut = 1234ul;
            swap.amountSrcOut = "0";
            swap.totalSupply = "100";
            var swapLog = new SwapLog(swap, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");
            var transactionLogs = new List<TransactionLog> { approvalLog, transferLog, swapLog };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionLogsByTransactionIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(transactionLogs);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Block(1, "blockHash", DateTime.UtcNow, DateTime.UtcNow));

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _eventsAssemblerMock.Verify(callTo => callTo.Assemble(transactionLogs), Times.Once());
        }
    }
}
