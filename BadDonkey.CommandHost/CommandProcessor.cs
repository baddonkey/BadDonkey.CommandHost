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
            _commands.Reset();

            while (_commands.MoveNext())
            {
                using var lifetimeScope = AutoFacContainerProvider.Container.BeginLifetimeScope();

                var command = _commands.Current;

                if (command == null) 
                    continue;

                var handler = lifetimeScope.ResolveNamed<ICommandHandler>(command.GetType().Name.Split("Command").First());
                ((dynamic)handler).Run();
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
