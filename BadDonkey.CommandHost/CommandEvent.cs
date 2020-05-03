using System;

namespace BadDonkey.CommandHost
{
    public class CommandEvent : EventArgs
    {
        public Command Command { get; set; }
    }
}
