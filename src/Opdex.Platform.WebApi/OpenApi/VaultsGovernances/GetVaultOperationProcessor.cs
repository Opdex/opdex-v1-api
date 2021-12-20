using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Opdex.Platform.Common.Models;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.VaultsGovernances;

public class GetVaultOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var vaultAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        vaultAddressParameter.DefineAsNetworkAddress(new Address("tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i"));

        return true;
    }
}
