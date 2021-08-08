using AutoMapper;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
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
        private readonly Mock<IModelAssembler<SwapLog, SwapEventDto>> _swapEventDtoAssembler;
        private readonly Mock<IModelAssembler<MintLog, AddLiquidityEventDto>> _mintProvideEventDtoAssembler;
        private readonly Mock<IModelAssembler<BurnLog, RemoveLiquidityEventDto>> _burnProvideEventDtoAssembler;
        private readonly Mock<IModelAssembler<TransferLog, TransferEventDto>> _transferEventDtoAssembler;
        private readonly Mock<IModelAssembler<ApprovalLog, ApprovalEventDto>> _approvalEventDtoAssembler;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly TransactionDtoAssembler _handler;

        public TransactionDtoAssemblerTests()
        {
            _swapEventDtoAssembler = new Mock<IModelAssembler<SwapLog, SwapEventDto>>();
            _mintProvideEventDtoAssembler = new Mock<IModelAssembler<MintLog, AddLiquidityEventDto>>();
            _burnProvideEventDtoAssembler = new Mock<IModelAssembler<BurnLog, RemoveLiquidityEventDto>>();
            _transferEventDtoAssembler = new Mock<IModelAssembler<TransferLog, TransferEventDto>>();
            _approvalEventDtoAssembler = new Mock<IModelAssembler<ApprovalLog, ApprovalEventDto>>();
            _mediatorMock = new Mock<IMediator>();

            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();

            _handler = new TransactionDtoAssembler(_mediatorMock.Object, mapper, _swapEventDtoAssembler.Object, _mintProvideEventDtoAssembler.Object,
                                                   _burnProvideEventDtoAssembler.Object, _transferEventDtoAssembler.Object,
                                                   _approvalEventDtoAssembler.Object);
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
        public async Task AssembleTransactionDto_Sends_SwapEventDtoAssembler()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "to";
            txLog.amountCrsIn = 0ul;
            txLog.amountSrcIn = "12";
            txLog.amountCrsOut = 1234ul;
            txLog.amountSrcOut = "0";
            txLog.totalSupply = "100";
            var swapLog = new SwapLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionLogsByTransactionIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TransactionLog>{swapLog});

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Block(1, "blockHash", DateTime.UtcNow, DateTime.UtcNow));

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _swapEventDtoAssembler.Verify(callTo => callTo.Assemble(swapLog), Times.Once());
        }

        [Fact]
        public async Task AssembleTransactionDto_Sends_MintEventDtoAssembler()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "sender";
            txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amountCrs = 1234ul;
            txLog.amountSrc = "83475";
            txLog.amountLpt = "23423";
            txLog.totalSupply = "10";
            var mintLog = new MintLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionLogsByTransactionIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TransactionLog>{mintLog});

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Block(1, "blockHash", DateTime.UtcNow, DateTime.UtcNow));

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _mintProvideEventDtoAssembler.Verify(callTo => callTo.Assemble(mintLog), Times.Once());
        }

        [Fact]
        public async Task AssembleTransactionDto_Sends_BurnEventDtoAssembler()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "sender";
            txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amountCrs = 1234ul;
            txLog.amountSrc = "83475";
            txLog.amountLpt = "23423";
            txLog.totalSupply = "100";
            var burnLog = new BurnLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionLogsByTransactionIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TransactionLog>{burnLog});

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Block(1, "blockHash", DateTime.UtcNow, DateTime.UtcNow));

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _burnProvideEventDtoAssembler.Verify(callTo => callTo.Assemble(burnLog), Times.Once());
        }

        [Fact]
        public async Task AssembleTransactionDto_Sends_TransferEventDtoAssembler()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "to";
            txLog.amount = "32981";
            var transferLog = new TransferLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionLogsByTransactionIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TransactionLog>{transferLog});

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Block(1, "blockHash", DateTime.UtcNow, DateTime.UtcNow));

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _transferEventDtoAssembler.Verify(callTo => callTo.Assemble(transferLog), Times.Once());
        }

        [Fact]
        public async Task AssembleTransactionDto_Sends_ApproveEventDtoAssembler()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.spender = "spender";
            txLog.amount = "1";
            txLog.oldAmount = "0";
            var approvalLog = new ApprovalLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
            var transaction = new Transaction(1, "txHash", 2, 3, "from", "to", true, "newContractAddress");

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTransactionLogsByTransactionIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<TransactionLog>{approvalLog});

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveBlockByHeightQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Block(1, "blockHash", DateTime.UtcNow, DateTime.UtcNow));

            // Act
            await _handler.Assemble(transaction);

            // Assert
            _approvalEventDtoAssembler.Verify(callTo => callTo.Assemble(approvalLog), Times.Once());
        }
    }
}
