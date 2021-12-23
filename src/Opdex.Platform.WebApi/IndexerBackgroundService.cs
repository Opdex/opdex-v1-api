using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Indexer;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Domain.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Common.Exceptions;

namespace Opdex.Platform.WebApi;

public class IndexerBackgroundService : BackgroundService
{
    private readonly ILogger<IndexerBackgroundService> _logger;
    private readonly IOptionsMonitor<IndexerConfiguration> _indexerOptions;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OpdexConfiguration _opdexConfiguration;
    private const string IndexingAlreadyRunningLog = "Index already running.";

    public IndexerBackgroundService(IServiceScopeFactory scopeFactory,
                                    OpdexConfiguration opdexConfiguration,
                                    ILogger<IndexerBackgroundService> logger,
                                    IOptionsMonitor<IndexerConfiguration> indexerOptions)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _opdexConfiguration = opdexConfiguration ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        _indexerOptions = indexerOptions ?? throw new ArgumentNullException(nameof(indexerOptions));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var started = false;
        var unavailable = false;

        IMediator mediator;
        await using var scope = _scopeFactory.CreateAsyncScope();
        {
            mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        }

        using (_logger.BeginScope("Indexer"))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (started)
                    {
                        // delay 30 seconds when indexing services are unavailable
                        // delay 8 to 12 seconds when indexing is available
                        var seconds = unavailable ? 30 : new Random().Next(8, 13);
                        await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
                    }

                    var indexLock = await mediator.Send(new RetrieveIndexerLockQuery(), cancellationToken);
                    if (!indexLock.Available || !_indexerOptions.CurrentValue.Enabled)
                    {
                        _logger.LogDebug("Indexing services unavailable.");
                        unavailable = true;
                        continue;
                    }

                    if (indexLock.Locked)
                    {
                        var isSameInstanceReprocessing = indexLock.Reason == IndexLockReason.Indexing &&
                                                         indexLock.InstanceId == _opdexConfiguration.InstanceId;

                        var totalSecondsLocked = DateTime.UtcNow.Subtract(indexLock.ModifiedDate).TotalSeconds;

                        if (!isSameInstanceReprocessing)
                        {
                            using (_logger.BeginScope(new Dictionary<string, object>()
                                   {
                                       {"TotalSecondsLocked", totalSecondsLocked}
                                   }))
                            {
                                _logger.LogWarning(IndexingAlreadyRunningLog);
                            }

                            continue;
                        }

                        using (_logger.BeginScope(new Dictionary<string, object>()
                               {
                                   { "TotalSecondsLocked", totalSecondsLocked },
                                   { "LockedReason", indexLock.Reason }
                               }))
                        {
                            _logger.LogWarning("Attempting to forcefully unlock indexer.");
                        }

                        // Consider a rewind after unlock.
                        // Rewind would go back to block prior to the previous locking timestamp to ensure all transactions and blocks were processed
                        await mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);
                    }

                    // The latest synced block we have, if we don't have any, the tip of Cirrus chain, else null
                    var bestBlock = await mediator.Send(new GetBestBlockReceiptQuery(), cancellationToken);
                    var lockReason = bestBlock is null ? IndexLockReason.Rewinding : IndexLockReason.Indexing;

                    var tryLock = await mediator.Send(new MakeIndexerLockCommand(lockReason), cancellationToken);
                    if (!tryLock)
                    {
                        _logger.LogWarning(IndexingAlreadyRunningLog);
                        continue;
                    }

                    try
                    {
                        if (lockReason == IndexLockReason.Rewinding)
                        {
                            bestBlock = await mediator.Send(new GetBlockReceiptAtChainSplitCommand(), CancellationToken.None);
                            var rewound = await mediator.Send(new CreateRewindToBlockCommand(bestBlock.Height), CancellationToken.None);
                            if (!rewound) throw new Exception($"Failure rewinding database to block height: {bestBlock.Height}");
                        }

                        await mediator.Send(new ProcessLatestBlocksCommand(bestBlock, _opdexConfiguration.Network), cancellationToken);
                    }
                    finally
                    {
                        await mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);
                    }

                    unavailable = false;
                }
                catch (TaskCanceledException)
                {
                    // shutdown occurred
                    _logger.LogWarning("Indexing cancelled");
                }
                catch(MaximumReorgException ex)
                {
                    _logger.LogCritical("Encountered a reorg which exceeds max reorg limit");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failure to process Cirrus blocks");
                }
                finally
                {
                    started = true;
                }
            }
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting indexer.");
        return base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Shutting down indexer.");

        await base.StopAsync(cancellationToken);

        try
        {
            IMediator mediator;
            await using var scope = _scopeFactory.CreateAsyncScope();
            {
                mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            }

            var indexLock = await mediator.Send(new RetrieveIndexerLockQuery(), CancellationToken.None);
            if (indexLock.Locked && indexLock.InstanceId == _opdexConfiguration.InstanceId)
            {
                await mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failure gracefully shutting down the indexer");
        }
    }
}
