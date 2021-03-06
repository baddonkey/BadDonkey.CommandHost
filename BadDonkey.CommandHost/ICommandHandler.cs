﻿using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BadDonkey.CommandHost
{
    public interface ICommandHandler
    {
        [UsedImplicitly]
        Task Run();

        ICommand Command { get; }
    }
}
