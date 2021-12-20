using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Opdex.Platform.Common.Models;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Tokens;

public class GetTokenHistoryOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var tokenAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        tokenAddressParameter.DefineAsNetworkAddressOrCrs(Address.Cirrus);

        return true;
    }
}
