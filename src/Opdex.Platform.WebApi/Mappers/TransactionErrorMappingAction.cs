using AutoMapper;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.WebApi.Models.Responses.Transactions;

namespace Opdex.Platform.WebApi.Mappers;

public class TransactionErrorMappingAction : IMappingAction<ITransactionDto, ITransactionResponseModel>
{
    private readonly ILoggerFactory _loggerFactory;

    public TransactionErrorMappingAction(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    public void Process(ITransactionDto source, ITransactionResponseModel destination, ResolutionContext context)
    {
        if (source.Error is null)
        {
            return;
        }

        var errorProcessor = new TransactionErrorProcessor(_loggerFactory.CreateLogger<TransactionErrorProcessor>());
        destination.Error = new TransactionErrorResponseModel
        {
            Friendly = errorProcessor.ProcessOpdexTransactionError(source.Error),
            Raw = source.Error
        };
    }
}
