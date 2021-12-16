using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Wallets;

public class GetMiningPositionByPoolOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var walletAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        walletAddressParameter.Schema.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        walletAddressParameter.Schema.MinLength = 30;
        walletAddressParameter.Schema.MaxLength = 42;

        var miningPoolAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "miningPool");
        miningPoolAddressParameter.Schema.Example = "tGeTZz3hnANsUre4GHB1Dy7c1EVxFjUHua";
        miningPoolAddressParameter.Schema.MinLength = 30;
        miningPoolAddressParameter.Schema.MaxLength = 42;

        return true;
    }
}
