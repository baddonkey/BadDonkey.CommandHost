using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BadDonkey.CommandHost
{
    public static class CommandHostExtensions
    {
        public static List<Command> GetCommandsFromAssembly(this JObject configuration, string key, Assembly commandAssembly)
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new CommandConverter(commandAssembly));
            return configuration[key].ToObject<List<Command>>(serializer);
        }

        public static IHostBuilder UseCommandHost(this IHostBuilder hostBuilder, string commandFile, string section, Assembly commandAssembly = null)
        {
            hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());

            var commands = JObject.Parse(File.ReadAllText(commandFile))
                .GetCommandsFromAssembly(section, commandAssembly).GetEnumerator();

            hostBuilder.ConfigureServices((hostContext, services) =>services.AddHostedService(p => p.GetService<ICommandProcessor>()));

            hostBuilder.ConfigureContainer<ContainerBuilder>((y, b) =>
            {
                var commandList = new CommandList();

                while(commands.MoveNext())
                {
                    var command = commands.Current;

                    if (command == null)
                        return;

                    commandList.Add(command);

                    b.Register(c => Convert.ChangeType(commandList.Current, command.GetType())).As(command.GetType());

                }

                b.RegisterBuildCallback(x => AutoFacContainerProvider.Container = x);
                
                b.Register(s => commandList).SingleInstance();

                b.RegisterType<CommandProcessor>().As<ICommandProcessor>().SingleInstance();

                if (commandAssembly != null)
                    b.RegisterAssemblyTypes(commandAssembly).Where(x => x.Name.EndsWith("Handler")).Named(s => s.Name.Split("Handler").First(), typeof(ICommandHandler));
            });

            return hostBuilder;
        }
    }
}
