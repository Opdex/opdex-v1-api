namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public enum ScOpcodeType : byte
{
    OP_CREATECONTRACT = 0xc0,
    OP_CALLCONTRACT = 0xc1,
    OP_SPEND = 0xc2,
    OP_INTERNALCONTRACTTRANSFER = 0xc3
}
