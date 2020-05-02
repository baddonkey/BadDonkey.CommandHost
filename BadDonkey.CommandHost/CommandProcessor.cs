using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace BadDonkey.CommandHost
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IEnumerator<Command> _commands;

        public CommandProcessor(IEnumerator<Command> commands)
        {
            _commands = commands;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            
#pragma warning disable 4014
            RunAll();
#pragma warning restore 4014
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task RunAll()
        {
            _commands.Reset();

            while (_commands.MoveNext())
            {
                await using var lifetimeScope = AutoFacContainerProvider.Container.BeginLifetimeScope();

                var command = _commands.Current;

                if (command == null)
                    continue;

                var handler = lifetimeScope.ResolveNamed<ICommandHandler>(command.GetType().Name.Split("Command").First());
                await((dynamic)handler).Run();
            }
        }

    }
}
