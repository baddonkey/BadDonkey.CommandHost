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

        public Task Run()
        {
            _logger.LogInformation($"Run: {_command.GetType().Name}: Host: {_command.Host}");
            return Task.CompletedTask;
        }
    }
}