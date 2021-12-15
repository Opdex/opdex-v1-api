using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.VaultsGovernances;

public class QuoteProposeMinimumVoteOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var vaultAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        vaultAddressParameter.Example = "tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i";
        vaultAddressParameter.MinLength = 30;
        vaultAddressParameter.MaxLength = 42;

        return true;
    }
}
