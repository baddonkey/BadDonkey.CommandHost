using System.Threading;
using System.Threading.Tasks;
using BadDonkey.CommandHost;
using Microsoft.Extensions.Logging;

namespace Samples.App
{
    public class SampleProcessor: ICommandProcessor
    {
        private readonly ILogger<SampleProcessor> _logger;
        private readonly ICommandProcessor _decorated;

        public SampleProcessor(ILogger<SampleProcessor> logger, ICommandProcessor decorated)
        {
            _logger = logger;
            _decorated = decorated;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SampleProcessor: StartAsync: start");

            var result = _decorated.StartAsync(cancellationToken);

            _logger.LogInformation("SampleProcessor: StartAsync: end");

            return result;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _decorated.StopAsync(cancellationToken);
        }
    }
}
