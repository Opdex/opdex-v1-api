using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Wallets;

public class GetStakingPositionByPoolOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var walletAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        walletAddressParameter.Schema.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        walletAddressParameter.Schema.MinLength = 30;
        walletAddressParameter.Schema.MaxLength = 42;

        var liquidityPoolAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "liquidityPool");
        liquidityPoolAddressParameter.Schema.Example = "tLrMcU1csbN7RxGjBMEnJeLoae3PxmQ9cr";
        liquidityPoolAddressParameter.Schema.MinLength = 30;
        liquidityPoolAddressParameter.Schema.MaxLength = 42;

        return true;
    }
}
