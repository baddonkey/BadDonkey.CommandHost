using System;

namespace BadDonkey.CommandHost
{
    public class CommandException
    {
        public string Kind { get; set; }

        public string Name { get; set; }

        public Exception Exception { get; set; }
    }
}
