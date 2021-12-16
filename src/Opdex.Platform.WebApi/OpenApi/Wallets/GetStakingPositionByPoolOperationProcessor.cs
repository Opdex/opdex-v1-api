using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Wallets;

public class GetStakingPositionByPoolOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var walletAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        walletAddressParameter.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        walletAddressParameter.MinLength = 30;
        walletAddressParameter.MaxLength = 42;

        var liquidityPoolAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "liquidityPool");
        liquidityPoolAddressParameter.Example = "tLrMcU1csbN7RxGjBMEnJeLoae3PxmQ9cr";
        liquidityPoolAddressParameter.MinLength = 30;
        liquidityPoolAddressParameter.MaxLength = 42;

        return true;
    }
}
