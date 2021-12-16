using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Wallets;

public class GetMiningPositionByPoolOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var walletAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        walletAddressParameter.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        walletAddressParameter.MinLength = 30;
        walletAddressParameter.MaxLength = 42;

        var miningPoolAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "miningPool");
        miningPoolAddressParameter.Example = "tGeTZz3hnANsUre4GHB1Dy7c1EVxFjUHua";
        miningPoolAddressParameter.MinLength = 30;
        miningPoolAddressParameter.MaxLength = 42;

        return true;
    }
}
