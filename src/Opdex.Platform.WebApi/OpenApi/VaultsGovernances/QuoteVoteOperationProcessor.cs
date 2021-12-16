using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.VaultsGovernances;

public class QuoteVoteOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var vaultAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        vaultAddressParameter.Schema.Example = "tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i";
        vaultAddressParameter.Schema.MinLength = 30;
        vaultAddressParameter.Schema.MaxLength = 42;

        var proposalIdParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "proposalId");
        proposalIdParameter.Schema.Minimum = 1;
        proposalIdParameter.Schema.Example = 5;

        return true;
    }
}
