using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Opdex.Platform.Common.Models;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.MiningPools;

public class CollectMiningRewardsOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var addressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        addressParameter.DefineAsNetworkAddress(new Address("tGeTZz3hnANsUre4GHB1Dy7c1EVxFjUHua"));

        return true;
    }
}
