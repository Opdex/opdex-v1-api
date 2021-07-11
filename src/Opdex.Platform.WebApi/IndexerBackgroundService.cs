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
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;

namespace Opdex.Platform.WebApi
{
    public class IndexerBackgroundService : BackgroundService
    {
        private readonly ILogger<IndexerBackgroundService> _logger;
        private readonly IServiceProvider _services;
        private readonly NetworkType _network;

        public IndexerBackgroundService(IServiceProvider services, OpdexConfiguration opdexConfiguration, ILogger<IndexerBackgroundService> logger)
        {
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var started = false;
            var unavailable = false;

            using var scope = _services.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

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
                        throw new IndexingAlreadyRunningException();
                    }

                    await mediator.Send(new MakeIndexerLockCommand());

                    await mediator.Send(new ProcessLatestBlocksCommand(_network), cancellationToken);

                    await mediator.Send(new MakeIndexerUnlockCommand());

                    unavailable = false;
                }
                catch (IndexingAlreadyRunningException ex)
                {
                    _logger.LogError(ex, "Indexing already running");
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

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
    }
}
