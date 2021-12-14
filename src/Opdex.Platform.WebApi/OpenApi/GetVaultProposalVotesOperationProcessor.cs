using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi;

public class GetVaultProposalVotesOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var vaultAddressParameter = context.OperationDescription.Operation.Parameters.FirstOrDefault(parameter => parameter.Name == "address");
        if (vaultAddressParameter is not null)
        {
            vaultAddressParameter.Example = "tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i";
        }

        var proposalIdParameter = context.OperationDescription.Operation.Parameters.FirstOrDefault(parameter => parameter.Name == "proposalId");
        if (proposalIdParameter is not null)
        {
            proposalIdParameter.Minimum = 1;
            proposalIdParameter.Example = 5;
        }

        var pledgerParameter = context.OperationDescription.Operation.Parameters.FirstOrDefault(parameter => parameter.Name == "pledger");
        if (pledgerParameter is not null)
        {
            pledgerParameter.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        }

        var voterParameter = context.OperationDescription.Operation.Parameters.FirstOrDefault(parameter => parameter.Name == "voter");
        if (voterParameter is not null)
        {
            voterParameter.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        }

        return true;
    }
}
