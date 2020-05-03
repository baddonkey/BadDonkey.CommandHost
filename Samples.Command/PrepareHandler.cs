using System.Threading.Tasks;
using BadDonkey.CommandHost;
using Microsoft.Extensions.Logging;

namespace Samples.Commands
{
    public class PrepareHandler : ICommandHandler
    {
        private readonly ILogger<PrepareHandler> _logger;
        private readonly PrepareCommand _command;

        public PrepareHandler(ILogger<PrepareHandler> logger, PrepareCommand command)
        {
            _logger = logger;
            _command = command;
        }

        public Task Run()
        {
            _logger.LogInformation($"PrepareHandler.Run: do nothing: {_command.Name}");
            return Task.CompletedTask;
        }
    }
}
