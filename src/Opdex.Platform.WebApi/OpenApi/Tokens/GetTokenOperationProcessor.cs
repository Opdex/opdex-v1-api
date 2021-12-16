using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Tokens;

public class GetTokenOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var tokenAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        tokenAddressParameter.Schema.Example = "tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3";
        tokenAddressParameter.Schema.MinLength = 30;
        tokenAddressParameter.Schema.MaxLength = 42;

        return true;
    }
}
