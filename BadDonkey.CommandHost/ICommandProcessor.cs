using System;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace BadDonkey.CommandHost
{
    public interface ICommandProcessor : IHostedService
    {
        event EventHandler CommandsProcessed;
        event EventHandler<CommandExceptionEvent> CommandException;
        event EventHandler<CommandEvent> CommandStart;

        IReadOnlyList<CommandException> CommandExceptions { get; }
    }
}