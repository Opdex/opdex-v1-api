using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Wallets;

public class GetAddressBalanceByTokenOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var walletAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        walletAddressParameter.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        walletAddressParameter.MinLength = 30;
        walletAddressParameter.MaxLength = 42;

        var tokenAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "token");
        tokenAddressParameter.Example = "tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3";
        tokenAddressParameter.MinLength = 30;
        tokenAddressParameter.MaxLength = 42;

        return true;
    }
}
