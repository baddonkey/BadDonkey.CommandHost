using System;

namespace BadDonkey.CommandHost
{
    public class CommandExceptionEvent : EventArgs
    {
        public CommandException CommandException { get; set; }
    }
}
