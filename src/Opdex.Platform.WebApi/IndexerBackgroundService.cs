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
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi;

public class IndexerBackgroundService : BackgroundService
{
    private readonly ILogger<IndexerBackgroundService> _logger;
    private readonly IServiceProvider _services;
    private readonly OpdexConfiguration _opdexConfiguration;

    private const string IndexingAlreadyRunningLog = "Index already running.";

    public IndexerBackgroundService(IServiceProvider services, OpdexConfiguration opdexConfiguration, ILogger<IndexerBackgroundService> logger)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _opdexConfiguration = opdexConfiguration ?? throw new ArgumentNullException(nameof(opdexConfiguration));
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var started = false;
        var unavailable = false;

        IMediator mediator;
        using var scope = _services.CreateScope();
        {
            mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        }

        using (_logger.BeginScope("Indexer"))
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (started)
                {
                    // delay 30 seconds when indexing services are unavailable (db flag off)
                    // delay 8 to 12 seconds when indexing is available
                    var seconds = unavailable ? 30 : new Random().Next(8, 13);

                    await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
                }

                try
                {
                    var indexLock = await mediator.Send(new RetrieveIndexerLockQuery(), cancellationToken);
                    if (!indexLock.Available)
                    {
                        _logger.LogWarning("Indexing services unavailable");
                        unavailable = true;
                        continue;
                    }

                    if (indexLock.Locked)
                    {
                        var stuckDuringRewind = indexLock.Reason == IndexLockReason.Rewind && indexLock.ModifiedDate.AddMinutes(1) < DateTime.UtcNow;
                        var isSameInstanceReprocessing = indexLock.Reason == IndexLockReason.Index && indexLock.InstanceId == _opdexConfiguration.InstanceId;

                        var totalSecondsLocked = DateTime.UtcNow.Subtract(indexLock.ModifiedDate).TotalSeconds;

                        if (!stuckDuringRewind && !isSameInstanceReprocessing)
                        {
                            using (_logger.BeginScope(new Dictionary<string, object>()
                            {
                                { "TotalSecondsLocked", totalSecondsLocked }
                            }))
                            {
                                _logger.LogWarning(IndexingAlreadyRunningLog);
                            }
                            continue;
                        }

                        // Consider a rewind after unlock.
                        // Rewind would go back to block prior to the previous locking timestamp to ensure all transactions and blocks were processed
                        await mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);

                        using (_logger.BeginScope(new Dictionary<string, object>()
                            {
                                { "TotalSecondsLocked", totalSecondsLocked },
                                { "LockedReason", indexLock.Reason }
                            }))
                        {
                            _logger.LogWarning("Indexer forcefully unlocked.");
                        }
                    }

                    await mediator.Send(new MakeIndexerLockCommand(IndexLockReason.Index), CancellationToken.None);

                    try
                    {
                        await mediator.Send(new ProcessLatestBlocksCommand(_opdexConfiguration.Network), cancellationToken);
                    }
                    finally
                    {
                        await mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);
                    }

                    unavailable = false;
                }
                catch (IndexingAlreadyRunningException ex)
                {
                    _logger.LogError(ex, IndexingAlreadyRunningLog);
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

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var indexLock = await mediator.Send(new RetrieveIndexerLockQuery(), CancellationToken.None);
            if (indexLock.Locked && indexLock.InstanceId == _opdexConfiguration.InstanceId)
            {
                await mediator.Send(new MakeIndexerUnlockCommand(), CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failure gracefully shutting down the indexer, indexing locked");
        }
        finally
        {
            await base.StopAsync(CancellationToken.None);
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        return base.StartAsync(cancellationToken);
    }
}
