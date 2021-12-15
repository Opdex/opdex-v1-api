using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.VaultsGovernances;

public class GetVoteOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var vaultAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        vaultAddressParameter.Example = "tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i";
        vaultAddressParameter.MinLength = 30;
        vaultAddressParameter.MaxLength = 42;

        var proposalIdParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "proposalId");
        proposalIdParameter.Minimum = 1;
        proposalIdParameter.Example = 5;

        var voterParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "voter");
        voterParameter.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        voterParameter.MinLength = 30;
        voterParameter.MaxLength = 42;
        return true;
    }
}
