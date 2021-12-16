using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Wallets;

public class GetAllowanceOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var walletAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "address");
        walletAddressParameter.Schema.Example = "tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm";
        walletAddressParameter.Schema.MinLength = 30;
        walletAddressParameter.Schema.MaxLength = 42;

        var tokenAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "token");
        tokenAddressParameter.Schema.Example = "tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3";
        tokenAddressParameter.Schema.MinLength = 30;
        tokenAddressParameter.Schema.MaxLength = 42;

        var spenderAddressParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "spender");
        spenderAddressParameter.Schema.Example = "t8XpH1pNYDgCnqk91ZQKLgpUVeJ7XmomLT";
        spenderAddressParameter.Schema.MinLength = 30;
        spenderAddressParameter.Schema.MaxLength = 42;

        return true;
    }
}
