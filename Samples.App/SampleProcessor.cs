using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BadDonkey.CommandHost;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Samples.App
{
    public class SampleProcessor: ICommandProcessor
    {
        private readonly ILogger<SampleProcessor> _logger;
        private readonly ICommandProcessor _decorated;

        public SampleProcessor(ILogger<SampleProcessor> logger, ICommandProcessor decorated, IHostApplicationLifetime hostLifeTime)
        {
            _logger = logger;
            _decorated = decorated;

            _decorated.CommandsProcessed += (sender, args) => hostLifeTime.StopApplication();
            _decorated.CommandException += (sender, e) =>_logger.LogError(e.CommandException.Exception, $"Error on command {e.CommandException.Name}: {e.CommandException.Exception.Message}");
            _decorated.CommandStart += (sender, e) => _logger.LogInformation($"Start command {e.Command.Name}");
        }

        public IReadOnlyList<CommandException> CommandExceptions => _decorated.CommandExceptions;

        public event EventHandler CommandsProcessed
        {
            add => _decorated.CommandsProcessed += value;

            remove => _decorated.CommandsProcessed -= value;
        }

        public event EventHandler<CommandExceptionEvent> CommandException
        {
            add => _decorated.CommandException += value;

            remove => _decorated.CommandException -= value;
        }

        public event EventHandler<CommandEvent> CommandStart
        {
            add => _decorated.CommandStart += value;

            remove => _decorated.CommandStart -= value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SampleProcessor: StartAsync: start");

            var result = _decorated.StartAsync(cancellationToken);

            return result;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SampleProcessor: StopAsync: stop");

            return _decorated.StopAsync(cancellationToken);
        }
    }
}
