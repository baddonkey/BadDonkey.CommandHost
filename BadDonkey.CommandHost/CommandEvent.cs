using System;

namespace BadDonkey.CommandHost
{
    public class CommandEvent : EventArgs
    {
        public ICommand Command { get; set; }
    }
}
