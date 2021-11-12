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

namespace Opdex.Platform.WebApi
{
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
                            if (indexLock.InstanceId != _opdexConfiguration.InstanceId)
                            {
                                _logger.LogWarning(IndexingAlreadyRunningLog);
                                continue;
                            }
                            
                            // Todo: If this is somehow the "fix" for the locking indexer bug seen occasionally consider a rewind after unlock
                            // Rewind would go back to block prior to the previous locking timestamp to ensure all transactions and blocks were processed
                            await mediator.Send(new MakeIndexerUnlockCommand());
                            _logger.LogWarning("Indexer forcefully unlocked");
                        }

                        await mediator.Send(new MakeIndexerLockCommand());

                        try
                        {
                            await mediator.Send(new ProcessLatestBlocksCommand(_opdexConfiguration.Network), cancellationToken);
                        }
                        finally
                        {
                            await mediator.Send(new MakeIndexerUnlockCommand());
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

                var indexLock = await mediator.Send(new RetrieveIndexerLockQuery());
                if (indexLock.Locked && indexLock.InstanceId == _opdexConfiguration.InstanceId)
                {
                    await mediator.Send(new MakeIndexerUnlockCommand());
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Failure gracefully shutting down the indexer, indexing locked");
            }
            finally
            {
                await base.StopAsync(cancellationToken);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
    }
}
