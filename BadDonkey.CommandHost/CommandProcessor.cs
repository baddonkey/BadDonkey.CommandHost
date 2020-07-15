using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;

namespace BadDonkey.CommandHost
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly List<CommandException> _commandCommandExceptions = new List<CommandException>();
        private readonly CommandList _commandList;


        public CommandProcessor(CommandList commandList)
        {
            _commandList = commandList;
        }

        public IReadOnlyList<CommandException> CommandExceptions => _commandCommandExceptions;

        public event EventHandler CommandsProcessed;
        public event EventHandler<CommandExceptionEvent> CommandException;
        public event EventHandler<CommandEvent> CommandStart;

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

        protected virtual void OnCommandStart(ICommand command)
        {
            var commandEvent = new CommandEvent
            {
                Command = command
            };

            var handler = CommandStart;
            handler?.Invoke(this, commandEvent);
        }

        protected virtual void OnCommandProcessed()
        {
            var handler = CommandsProcessed;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnCommandException(CommandException commandException)
        {
            _commandCommandExceptions.Add(commandException);

            var handler = CommandException;

            var commandExceptionEvent = new CommandExceptionEvent
            {
                CommandException = commandException
            };

            handler?.Invoke(this, commandExceptionEvent);
        }

        private  async Task RunAll()
        {
            _commandList.Reset();

            while (_commandList.Next())
            {
                await Run(_commandList.Current);
            }

            OnCommandProcessed();
        }

        private async Task Run(ICommand command)
        {
            await using var lifetimeScope = AutoFacContainerProvider.Container.BeginLifetimeScope();

            try
            {
                if (command == null)
                    return;

                var handler = lifetimeScope.ResolveNamed<ICommandHandler>(command.GetType().Name.Split("Command").First());

                
                OnCommandStart(command);

                await ((dynamic) handler).Run();
            }
            catch (Exception ex)
            {
                OnCommandException(new CommandException
                {
                    Name = _commandList.Current?.Name,
                    Kind = _commandList.Current?.GetType().Name,
                    Exception = ex
                });
            }
        }
    }
}
