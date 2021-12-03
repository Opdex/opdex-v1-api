using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Summaries;

public class PersistTokenSummaryCommandHandler : IRequestHandler<PersistTokenSummaryCommand, ulong>
{
    private static readonly string InsertSqlCommand =
        $@"INSERT INTO token_summary (
                {nameof(TokenSummaryEntity.MarketId)},
                {nameof(TokenSummaryEntity.TokenId)},
                {nameof(TokenSummaryEntity.DailyPriceChangePercent)},
                {nameof(TokenSummaryEntity.PriceUsd)},
                {nameof(TokenSummaryEntity.CreatedBlock)},
                {nameof(TokenSummaryEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(TokenSummaryEntity.MarketId)},
                @{nameof(TokenSummaryEntity.TokenId)},
                @{nameof(TokenSummaryEntity.DailyPriceChangePercent)},
                @{nameof(TokenSummaryEntity.PriceUsd)},
                @{nameof(TokenSummaryEntity.CreatedBlock)},
                @{nameof(TokenSummaryEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID();".RemoveExcessWhitespace();

    private static readonly string UpdateSqlCommand =
        $@"UPDATE token_summary
                SET
                    {nameof(TokenSummaryEntity.DailyPriceChangePercent)} = @{nameof(TokenSummaryEntity.DailyPriceChangePercent)},
                    {nameof(TokenSummaryEntity.PriceUsd)} = @{nameof(TokenSummaryEntity.PriceUsd)},
                    {nameof(TokenSummaryEntity.ModifiedBlock)} = @{nameof(TokenSummaryEntity.ModifiedBlock)}
                WHERE {nameof(TokenSummaryEntity.Id)} = @{nameof(TokenSummaryEntity.Id)};".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PersistTokenSummaryCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistTokenSummaryCommandHandler> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ulong> Handle(PersistTokenSummaryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<TokenSummaryEntity>(request.TokenSummary);

            var isUpdate = entity.Id >= 1;

            var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

            var command = DatabaseQuery.Create(sql, entity, cancellationToken);

            var result = await _context.ExecuteScalarAsync<ulong>(command);

            return isUpdate ? entity.Id : result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to persist {nameof(TokenSummary)}");
            return 0;
        }
    }
}