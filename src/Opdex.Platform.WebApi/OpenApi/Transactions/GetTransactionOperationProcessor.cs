using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;
using Opdex.Platform.Common.Models;
using System.Linq;

namespace Opdex.Platform.WebApi.OpenApi.Transactions;

public class GetTransactionOperationProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var hashParameter = context.OperationDescription.Operation.Parameters.First(parameter => parameter.Name == "hash");
        hashParameter.DefineAsSha256(Sha256.Parse("7f98c57519e06e98ad96b2abb639ed4f2ecbd9158bd581837a187f129bde8bf9"));

        return true;
    }
}
