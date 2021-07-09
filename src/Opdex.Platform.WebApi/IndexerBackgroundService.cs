using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;
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

            while (!cancellationToken.IsCancellationRequested)
            {
                if (started)
                {
                    var seconds  = new Random().Next(8, 13); // 8 to 12 seconds

                    await Task.Delay(TimeSpan.FromSeconds(seconds), cancellationToken);
                }

                try
                {
                    using var scope = _services.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send(new MakeIndexerLockCommand());

                    await mediator.Send(new ProcessLatestBlocksCommand(_network), cancellationToken);

                    // Todo: This is going to lock on failure, maybe that's a good thing
                    await mediator.Send(new MakeIndexerUnlockCommand());
                }
                catch (IndexingAlreadyRunningException ex)
                {
                    _logger.LogInformation(ex, "Indexer already running");
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
