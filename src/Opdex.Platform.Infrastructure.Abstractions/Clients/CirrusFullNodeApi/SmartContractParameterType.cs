namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public enum SmartContractParameterType : uint
    {
        Unknown = 0,
        Boolean = 1,
        Byte = 2,
        Char = 3,
        String = 4,
        Uint32 = 5,
        Int32 = 6,
        UInt64 = 7,
        Int64 = 8,
        Address = 9,
        ByteArray = 10,
        UInt128 = 11,
        UInt256 = 12
    }
}