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

namespace Opdex.Platform.WebApi;

public class IndexerBackgroundService : BackgroundService
{
    private readonly ILogger<IndexerBackgroundService> _logger;
    private readonly IServiceProvider _services;
    private readonly OpdexConfiguration _opdexConfiguration;
    private const string IndexingAlreadyRunningLog = "Index already running.";

    public IndexerBackgroundService(IServiceProvider services,
                                    OpdexConfiguration opdexConfiguration,
                                    ILogger<IndexerBackgroundService> logger)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _opdexConfiguration = opdexConfiguration ?? throw new ArgumentNullException(nameof(opdexConfiguration));
    }

    public bool Running { get; private set; }

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
                        var isSameInstanceReprocessing = indexLock.Reason == IndexLockReason.Index && indexLock.InstanceId == _opdexConfiguration.InstanceId;

                        var totalSecondsLocked = DateTime.UtcNow.Subtract(indexLock.ModifiedDate).TotalSeconds;

                        if (!isSameInstanceReprocessing)
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

                    var tryLock = await mediator.Send(new MakeIndexerLockCommand(IndexLockReason.Index), CancellationToken.None);
                    if (!tryLock)
                    {
                        _logger.LogWarning(IndexingAlreadyRunningLog);
                        continue;
                    }

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
        _logger.LogDebug("Shutting down indexer.");
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
            await base.StopAsync(cancellationToken);
            Running = false;
        }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        if (Running) return Task.CompletedTask;

        _logger.LogDebug("Starting indexer.");
        Running = true;

        return base.StartAsync(cancellationToken);
    }
}

/// <summary>
/// Manages the lifetime of <see cref="IndexerBackgroundService" />.
/// </summary>
/// <remarks>
/// This makes it much easier to handle graceful shutdown by the host. Stopping a hosted service cancels the token that started it. In the case of
/// the <see cref="Host" />, cancellation of the token is handled as an error. This can be ignored, but then if the hosted service is restarted,
/// the cancellation token is no longer linked to the application lifetime.
/// </remarks>
public sealed class IndexerBackgroundServiceManager : IHostedService, IDisposable
{
    private readonly IndexerBackgroundService _indexer;
    private readonly bool _runOnStartup;
    private readonly IDisposable _optionsMonitor;

    public IndexerBackgroundServiceManager(IServiceProvider services, OpdexConfiguration opdexConfiguration, ILogger<IndexerBackgroundService> logger,
                                           IOptionsMonitor<IndexerConfiguration> indexerOptions)
    {
        _indexer = new IndexerBackgroundService(services, opdexConfiguration, logger);
        _runOnStartup = indexerOptions.CurrentValue.Enabled;
        _optionsMonitor = indexerOptions.OnChange(async config =>
        {
            if (config.Enabled && !_indexer.Running) await _indexer.StartAsync(CancellationToken.None);
            else if (!config.Enabled && _indexer.Running) await _indexer.StopAsync(CancellationToken.None);
        });
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_runOnStartup) return;
        await _indexer.StartAsync(CancellationToken.None);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_indexer.Running) await _indexer.StopAsync(cancellationToken);
    }

    public void Dispose()
    {
        _optionsMonitor.Dispose();
    }
}

public class IndexerConfiguration
{
    public bool Enabled { get; set; }
}