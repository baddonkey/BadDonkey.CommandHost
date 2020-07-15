using System;
using System.Threading.Tasks;
using BadDonkey.CommandHost;
using Microsoft.Extensions.Logging;

namespace Samples.Commands
{
    public class AcmeHandler : ICommandHandler
    {
        private readonly ILogger<AcmeHandler> _logger;
        private readonly AcmeCommand _command;

        public AcmeHandler(ILogger<AcmeHandler> logger, AcmeCommand command)
        {
            _logger = logger;
            _command = command;
        }

        public async Task Run()
        {
            _logger.LogInformation($"Snooze ... {_command.Host}");
            await Task.Delay(TimeSpan.FromSeconds(10));

            throw new Exception("Sample Exception");
        }

        public ICommand Command => _command;
    }
}