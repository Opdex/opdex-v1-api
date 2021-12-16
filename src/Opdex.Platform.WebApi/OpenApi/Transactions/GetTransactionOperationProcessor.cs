using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Transactions;

public class GetTransactionOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var hashParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "hash");
        hashParameter.Schema.Example = "7f98c57519e06e98ad96b2abb639ed4f2ecbd9158bd581837a187f129bde8bf9";
        hashParameter.Schema.Pattern = @"^[A-Fa-f0-9]{64}$";
        hashParameter.Schema.MinLength = 64;
        hashParameter.Schema.MaxLength = 64;

        return true;
    }
}
