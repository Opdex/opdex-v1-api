namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public class ContractCodeDto
{
    public string Type { get; set; }
    public byte[] Bytecode { get; set; }
    public string CSharp { get; set; }
    public string Message { get; set; }
}